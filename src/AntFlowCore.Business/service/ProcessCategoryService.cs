using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

/// <summary>
/// 流程分类管理 业务实现. 对应 Java BpmProcessCategoryServiceImpl.
/// </summary>
public class ProcessCategoryService : IProcessCategoryService
{
    public IBpmProcessCategoryRepository _repository { get; }

    public ProcessCategoryService(IBpmProcessCategoryRepository repository)
    {
        _repository = repository;
    }

    public bool EditProcessCategory(BpmProcessCategoryVo vo)
    {
        if (vo.Id != null)
        {
            //编辑: 只更新非空字段
            BpmProcessCategory? exist = _repository.FirstOrDefault(a => a.Id == vo.Id.Value);
            if (exist == null)
            {
                throw new AFBizException("400010", "分类记录不存在");
            }
            if (!string.IsNullOrEmpty(vo.ProcessTypeName))
            {
                exist.ProcessTypeName = vo.ProcessTypeName;
            }
            _repository.Update(exist);
            return true;
        }

        //新增: 重名校验(process_type_name + is_app + is_del=0)
        int isApp = vo.IsApp ?? 0;
        if (_repository.Any(a => a.ProcessTypeName == vo.ProcessTypeName
                                 && a.IsApp == isApp && a.IsDel == 0))
        {
            throw new AFBizException("400011", "该选项名称已存在");
        }

        //sort = 当前 is_app 范围内最大 + 1
        List<BpmProcessCategory> sameApp = _repository.Find(a => a.IsApp == isApp && a.IsDel == 0);
        int nextSort = sameApp.Count == 0 ? 1 : sameApp.Max(a => a.Sort) + 1;

        BpmProcessCategory entity = new()
        {
            ProcessTypeName = vo.ProcessTypeName,
            IsApp = isApp,
            IsDel = 0,
            State = vo.State ?? 0,
            Sort = nextSort,
            Entrance = isApp == 1 ? "APP" : "PC",
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };
        _repository.Add(entity);
        return true;
    }

    public bool CategoryOperation(int type, long id)
    {
        switch (type)
        {
            case 2:
                MoveUp(id);
                break;
            case 3:
                MoveDown(id);
                break;
            case 4:
                //删除(软删, 演示环境前端已拦截, 后端保留能力)
                BpmProcessCategory? category = _repository.FirstOrDefault(a => a.Id == id);
                if (category == null)
                {
                    throw new AFBizException("400010", "分类记录不存在");
                }
                category.IsDel = 1;
                _repository.Update(category);
                break;
            default:
                throw new AFBizException("400012", "不支持的操作类型");
        }
        return true;
    }

    private void MoveUp(long id)
    {
        BpmProcessCategory current = GetOrThrow(id);
        int sort = current.Sort;
        if (sort <= 1)
        {
            throw new AFBizException("400013", "当前记录已到顶");
        }
        BpmProcessCategory? prev = _repository.FirstOrDefault(a => a.IsApp == current.IsApp
            && a.Sort == sort - 1 && a.IsDel == 0);
        if (prev != null)
        {
            prev.Sort = sort;
            _repository.Update(prev);
        }
        current.Sort = sort - 1;
        _repository.Update(current);
    }

    private void MoveDown(long id)
    {
        BpmProcessCategory current = GetOrThrow(id);
        int sort = current.Sort;
        int maxSort = _repository.Count(a => a.IsApp == current.IsApp && a.IsDel == 0);
        if (sort >= maxSort)
        {
            throw new AFBizException("400014", "当前记录已到底");
        }
        BpmProcessCategory? next = _repository.FirstOrDefault(a => a.IsApp == current.IsApp
            && a.Sort == sort + 1 && a.IsDel == 0);
        if (next != null)
        {
            next.Sort = sort;
            _repository.Update(next);
        }
        current.Sort = sort + 1;
        _repository.Update(current);
    }

    private BpmProcessCategory GetOrThrow(long id)
    {
        BpmProcessCategory? category = _repository.FirstOrDefault(a => a.Id == id);
        if (category == null)
        {
            throw new AFBizException("400010", "无此条记录");
        }
        return category;
    }

    public List<BpmProcessCategory> ProcessCategoryList(BpmProcessCategoryVo vo)
    {
        //条件式构建: IsApp 为空时不加 is_app 过滤(避免生成 NULL IS NULL 之类异常 SQL), 与 Java 对等
        var query = _repository.GetQueryable().Where(a => a.IsDel == 0);
        if (vo.IsApp != null)
        {
            query = query.Where(a => a.IsApp == vo.IsApp.Value);
        }
        List<BpmProcessCategory> list = query.ToList();
        list.Sort((x, y) => x.Sort.CompareTo(y.Sort));
        return list;
    }

    public ResultAndPage<BpmProcessCategoryVo> SelectPage(PageDto pageDto, BpmProcessCategoryVo vo)
    {
        Page<BpmProcessCategory> page = PageUtils.GetPageByPageDto<BpmProcessCategory>(pageDto);
        var query = _repository.GetQueryable().Where(a => a.IsDel == 0);
        if (!string.IsNullOrWhiteSpace(vo.ProcessTypeName))
        {
            string kw = vo.ProcessTypeName.Trim();
            query = query.Where(a => a.ProcessTypeName.Contains(kw));
        }
        List<BpmProcessCategory> records = query.OrderBy(a => a.Sort)
            .Skip((page.Current - 1) * page.Size).Take(page.Size).ToList();
        page.Total = query.Count();
        List<BpmProcessCategoryVo> vos = records.Select(c => new BpmProcessCategoryVo
        {
            Id = c.Id,
            ProcessTypeName = c.ProcessTypeName,
            IsDel = c.IsDel,
            Sort = c.Sort,
            IsApp = c.IsApp,
            State = c.State,
            Entrance = c.Entrance,
        }).ToList();
        return PageUtils.GetResultAndPage(vos, PageUtils.GetPageDto(page));
    }

    public List<BpmProcessCategoryVo> Options()
    {
        return ProcessCategoryList(new BpmProcessCategoryVo())
            .Select(c => new BpmProcessCategoryVo
            {
                Id = c.Id,
                ProcessTypeName = c.ProcessTypeName,
            })
            .ToList();
    }
}
