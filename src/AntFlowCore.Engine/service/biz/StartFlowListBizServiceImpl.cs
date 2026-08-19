using System.Collections;
using System.Reflection;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.adaptor.formoperation;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Engine.service.biz;

/// <summary>
/// 发起流程页聚合实现. 对应 Java StartFlowListBizServiceImpl.
/// 流程范围:DIY + LF + outside 全部可用流程(effective_status=1);未设计的 DIY 适配器并入「未分类」.
/// 权限:流程必须存在创建权限记录(permissions_type=2)才可见,从用户主体出发(全员/用户/部门及子部门/角色);
///       流程管理员跳过过滤展示全部(最高权限).
/// 布局:每栏 8 卡片位,标题占 1 位,超长分类块(&gt;7 个流程)整栏内滚;一页最多 3 栏,分类块不跨栏不跨页.
/// </summary>
public class StartFlowListBizServiceImpl : IStartFlowListBizService
{
    /// <summary>
    /// 每栏卡片位容量(含分类标题 1 位)
    /// </summary>
    private const int COLUMN_CAPACITY = 8;

    /// <summary>
    /// 每页最多栏数
    /// </summary>
    private const int MAX_COLUMN = 3;

    /// <summary>
    /// 未分类占位 key
    /// </summary>
    private const long UNCATEGORIZED_KEY = -1L;

    private readonly IBpmnConfRepository _bpmnConfRepository;
    private readonly IProcessCategoryService _processCategoryService;
    private readonly IProcessPermissionsRepository _processPermissionsRepository;
    private readonly IUserService _userService;
    private readonly IBpmnProcessAdminProvider? _bpmnProcessAdminProvider;

    public StartFlowListBizServiceImpl(
        IBpmnConfRepository bpmnConfRepository,
        IProcessCategoryService processCategoryService,
        IProcessPermissionsRepository processPermissionsRepository,
        IUserService userService,
        IBpmnProcessAdminProvider? bpmnProcessAdminProvider)
    {
        _bpmnConfRepository = bpmnConfRepository;
        _processCategoryService = processCategoryService;
        _processPermissionsRepository = processPermissionsRepository;
        _userService = userService;
        _bpmnProcessAdminProvider = bpmnProcessAdminProvider;
    }

    public ResultAndPage<StartFlowCategoryVo> Page(StartFlowListPageReq? req)
    {
        req ??= new StartFlowListPageReq();
        int pageNo = (req.Page == null || req.Page < 1) ? 1 : req.Page.Value;

        // 1. 聚合可用流程
        List<StartFlowListRowVo> rows = _bpmnConfRepository.SelectStartFlowList() ?? new List<StartFlowListRowVo>();
        FillDerivedType(rows);
        rows.AddRange(CollectUndesignedDiy(rows));

        // 2. 权限过滤(管理员跳过,展示全部)
        List<StartFlowListRowVo> allowed = FilterByPermission(rows);
        if (allowed.Count == 0)
        {
            return EmptyResult(pageNo);
        }

        // 3. 条件过滤(流程名称 > formCode > 流程类型)
        allowed = FilterByQuery(allowed, req);
        if (allowed.Count == 0)
        {
            return EmptyResult(pageNo);
        }

        // 4. 分类分组(分类按 sort asc,未分类最后;分类内按创建时间 asc)
        List<StartFlowCategoryVo> blocks = GroupByCategory(allowed);

        // 5. 栏切分
        List<List<StartFlowCategoryVo>> pages = SplitColumns(blocks);
        int pageCount = Math.Max(pages.Count, 1);
        int idx = pageNo - 1;
        List<StartFlowCategoryVo> current = (idx >= 0 && idx < pages.Count)
            ? pages[idx]
            : new List<StartFlowCategoryVo>();

        PageDto outDto = PageDto.BuildCountedPage(PageDtoOf(pageNo), blocks.Count);
        outDto.PageCount = pageCount;
        outDto.Page = pageNo;
        return new ResultAndPage<StartFlowCategoryVo>(current, outDto);
    }

    private ResultAndPage<StartFlowCategoryVo> EmptyResult(int pageNo)
    {
        PageDto outDto = PageDto.BuildCountedPage(PageDtoOf(pageNo), 0);
        outDto.PageCount = 1;
        outDto.Page = 1;
        return new ResultAndPage<StartFlowCategoryVo>(new List<StartFlowCategoryVo>(), outDto);
    }

    private static PageDto PageDtoOf(int pageNo)
    {
        return new PageDto { Page = pageNo, PageSize = 10 };
    }

    /// <summary>
    /// 条件过滤,优先级:流程名称 &gt; formCode &gt; 流程类型(命中前者忽略后者)
    /// </summary>
    private List<StartFlowListRowVo> FilterByQuery(List<StartFlowListRowVo> rows, StartFlowListPageReq req)
    {
        if (!string.IsNullOrWhiteSpace(req.BpmnName))
        {
            string kw = req.BpmnName.Trim();
            return rows.Where(r => r.BpmnName != null && r.BpmnName.Contains(kw)).ToList();
        }
        if (!string.IsNullOrWhiteSpace(req.FormCode))
        {
            string kw = req.FormCode.Trim();
            return rows.Where(r => r.FormCode != null && r.FormCode.Contains(kw)).ToList();
        }
        if (req.CategoryId != null)
        {
            if (req.CategoryId == -1L)
            {
                // 未分类: bpmn_type IS NULL
                return rows.Where(r => r.BpmnType == null).ToList();
            }
            return rows.Where(r => r.BpmnType != null && r.BpmnType.Value == req.CategoryId.Value).ToList();
        }
        return rows;
    }

    /// <summary>
    /// 派生 type:is_out_side_process=1 → OUTSIDE;is_lowcode_flow=1 → LF;否则 DIY
    /// </summary>
    private void FillDerivedType(List<StartFlowListRowVo> rows)
    {
        foreach (StartFlowListRowVo r in rows)
        {
            if (r.IsOutSideProcess == 1)
            {
                r.Type = "OUTSIDE";
            }
            else if (r.IsLowCodeFlow == 1)
            {
                r.Type = "LF";
            }
            else
            {
                r.Type = "DIY";
            }
        }
    }

    /// <summary>
    /// 收集未在 t_bpmn_conf 中出现的 DIY 适配器(未设计流程),并入未分类.
    /// .NET 侧通过开放泛型 IFormOperationAdaptor&lt;&gt; 扫描已注册适配器, 跳过 LF 适配器.
    /// </summary>
    private List<StartFlowListRowVo> CollectUndesignedDiy(List<StartFlowListRowVo> rows)
    {
        HashSet<string> existing = new(
            rows.Where(r => !string.IsNullOrEmpty(r.FormCode)).Select(r => r.FormCode!),
            StringComparer.Ordinal);
        List<StartFlowListRowVo> result = new();
        IEnumerable services = ServiceProviderUtils.GetServicesByOpenGenericType(typeof(IFormOperationAdaptor<>));
        foreach (object service in services)
        {
            DIYFormServiceAnnoAttribute? anno = service.GetType().GetCustomAttribute<DIYFormServiceAnnoAttribute>();
            if (anno == null || string.IsNullOrEmpty(anno.Desc) || string.IsNullOrEmpty(anno.SvcName))
            {
                continue;
            }
            // 跳过低代码适配器(SvcName="LF")
            if (anno.SvcName.Equals("LF", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            if (existing.Contains(anno.SvcName))
            {
                continue;
            }
            result.Add(new StartFlowListRowVo
            {
                FormCode = anno.SvcName,
                BpmnName = anno.Desc,
                Type = "DIY",
                CreateTime = null,
            });
        }
        return result;
    }

    /// <summary>
    /// 权限过滤:仅展示当前用户有创建权限(permissions_type=2)的流程.
    /// 管理员(provideProcessAdminInfo 命中)跳过过滤,展示全部(最高权限).
    /// 匹配方式(从主体出发):全员(object_type=4) ∪ 用户(object_type=1) ∪ 部门及子部门(object_type=2) ∪ 角色(object_type=3).
    /// </summary>
    private List<StartFlowListRowVo> FilterByPermission(List<StartFlowListRowVo> rows)
    {
        string userId = SecurityUtils.GetLogInEmpIdSafe();
        if (IsProcessAdmin(userId))
        {
            return rows;
        }
        HashSet<string> allowedKeys = GetAllowedCreateProcessKeys(userId);
        return rows.Where(r => r.FormCode != null && allowedKeys.Contains(r.FormCode)).ToList();
    }

    /// <summary>
    /// 判断当前用户是否为流程管理员
    /// </summary>
    private bool IsProcessAdmin(string userId)
    {
        if (_bpmnProcessAdminProvider == null || string.IsNullOrEmpty(userId))
        {
            return false;
        }
        BaseIdTranStruVo admin = _bpmnProcessAdminProvider.ProvideProcessAdminInfo();
        return admin != null && !string.IsNullOrEmpty(admin.Id) && userId == admin.Id;
    }

    /// <summary>
    /// 从用户主体出发,查询有创建权限(permissions_type=2)的 processKey 集合
    /// </summary>
    private HashSet<string> GetAllowedCreateProcessKeys(string userId)
    {
        List<BaseIdTranStruVo> depts = _userService.GetUserDepartmentsById(userId);
        List<BaseIdTranStruVo> roles = _userService.GetUserRolesById(userId);
        List<string> deptIds = depts.Where(d => !string.IsNullOrEmpty(d.Id)).Select(d => d.Id).ToList();
        List<string> roleIds = roles.Where(r => !string.IsNullOrEmpty(r.Id)).Select(r => r.Id).ToList();

        //与 Java 对等: 匹配条件下推到 DB(object_type 4/1/2/3 一组 OR), 避免全量拉取后内存过滤
        List<BpmProcessPermissions> perms = _processPermissionsRepository.GetQueryable()
            .Where(a => a.PermissionsType == 2 && a.IsDel == 0)
            .Where(a =>
                a.ObjectType == 4
                || (a.ObjectType == 1 && a.ObjectId == userId)
                || (deptIds.Count > 0 && a.ObjectType == 2 && deptIds.Contains(a.ObjectId))
                || (roleIds.Count > 0 && a.ObjectType == 3 && roleIds.Contains(a.ObjectId)))
            .ToList();

        var allowed = new HashSet<string>(StringComparer.Ordinal);
        foreach (BpmProcessPermissions p in perms)
        {
            if (!string.IsNullOrEmpty(p.ProcessKey))
            {
                allowed.Add(p.ProcessKey);
            }
        }
        return allowed;
    }

    /// <summary>
    /// 按分类分组:分类按 sort asc,未分类最后;分类内按创建时间 asc(null 最后)
    /// </summary>
    private List<StartFlowCategoryVo> GroupByCategory(List<StartFlowListRowVo> rows)
    {
        List<BpmProcessCategory> categories = _processCategoryService.ProcessCategoryList(new BpmProcessCategoryVo());

        var group = new Dictionary<long, List<StartFlowListRowVo>>();
        foreach (StartFlowListRowVo r in rows)
        {
            long key = r.BpmnType == null ? UNCATEGORIZED_KEY : r.BpmnType.Value;
            if (!group.TryGetValue(key, out List<StartFlowListRowVo>? list))
            {
                list = new List<StartFlowListRowVo>();
                group[key] = list;
            }
            list.Add(r);
        }

        var blocks = new List<StartFlowCategoryVo>();
        foreach (BpmProcessCategory c in categories)
        {
            if (!group.TryGetValue(c.Id, out List<StartFlowListRowVo>? fs) || fs.Count == 0)
            {
                continue;
            }
            group.Remove(c.Id);
            blocks.Add(BuildBlock(c.Id, c.ProcessTypeName, fs));
        }
        if (group.TryGetValue(UNCATEGORIZED_KEY, out List<StartFlowListRowVo>? uncategorized) && uncategorized.Count > 0)
        {
            blocks.Add(BuildBlock(null, "未分类", uncategorized));
        }
        return blocks;
    }

    private StartFlowCategoryVo BuildBlock(long? categoryId, string categoryName, List<StartFlowListRowVo> fs)
    {
        fs.Sort((x, y) =>
        {
            if (x.CreateTime == null && y.CreateTime == null)
            {
                return 0;
            }
            if (x.CreateTime == null)
            {
                return 1;
            }
            if (y.CreateTime == null)
            {
                return -1;
            }
            return x.CreateTime.Value.CompareTo(y.CreateTime.Value);
        });
        List<StartFlowCategoryVo.StartFlowVo> flows = fs.Select(r => new StartFlowCategoryVo.StartFlowVo
        {
            FormCode = r.FormCode,
            BpmnName = r.BpmnName,
            Type = r.Type,
            ApplicationId = r.ApplicationId,
            CreateTime = r.CreateTime,
        }).ToList();
        return new StartFlowCategoryVo
        {
            CategoryId = categoryId,
            CategoryName = categoryName,
            Flows = flows,
        };
    }

    /// <summary>
    /// 栏切分:按栏 1→2→3 顺序装,装不下换栏,三栏都装不下进下一页;分类块不跨栏不跨页
    /// </summary>
    private List<List<StartFlowCategoryVo>> SplitColumns(List<StartFlowCategoryVo> blocks)
    {
        var pages = new List<List<StartFlowCategoryVo>>();
        var currentPage = new List<StartFlowCategoryVo>();
        int[] used = new int[MAX_COLUMN];
        foreach (StartFlowCategoryVo block in blocks)
        {
            int size = BlockSize(block.Flows.Count);
            int col = -1;
            for (int i = 0; i < MAX_COLUMN; i++)
            {
                if (used[i] + size <= COLUMN_CAPACITY)
                {
                    col = i;
                    break;
                }
            }
            if (col == -1)
            {
                pages.Add(currentPage);
                currentPage = new List<StartFlowCategoryVo>();
                used = new int[MAX_COLUMN];
                col = 0;
            }
            used[col] += size;
            block.Column = col;
            currentPage.Add(block);
        }
        if (currentPage.Count > 0)
        {
            pages.Add(currentPage);
        }
        return pages;
    }

    /// <summary>
    /// 分类块占位:标题 1 位 + 流程卡片;超过栏容量则整栏内滚(占满一栏)
    /// </summary>
    private static int BlockSize(int flowCount)
    {
        return Math.Min(flowCount + 1, COLUMN_CAPACITY);
    }
}
