using System.Text.Json;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.constant;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;

using AntFlowCore.Persist.api.interf.repository;
using FreeSql;

namespace AntFlowCore.Business.service;

/// <summary>
/// 演示数据-业务数据 动态列表. 对应 Java DemoDataBusinessDataBizServiceImpl.
/// .NET 版无分表,逻辑与 Java 版一致.
/// </summary>
public class DemoDataBusinessDataService : IDemoDataBusinessDataService
{
    private const string MaskValue = "***";
    private const int MaxPageSize = 200;

    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IBpmnConfService _bpmnConfService;
    private readonly IBpmnConfLfFormdataFieldService _lfFormdataFieldService;
    private readonly ILFMainFieldService _lfMainFieldService;
    private readonly IBpmnNodeService _bpmnNodeService;
    private readonly IBpmVariableService _bpmVariableService;
    private readonly IBpmVariableMultiplayerService _bpmVariableMultiplayerService;
    private readonly IBpmVariableMultiplayerPersonnelService _bpmVariableMultiplayerPersonnelService;
    private readonly IBpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly IProcessPermissionsRepository _processPermissionsRepository;
    private readonly IRoleService _roleService;
    private readonly IBpmnProcessAdminProvider _bpmnProcessAdminProvider;
    private readonly IUserRepository _userRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IFreeSql _freeSql;

    public DemoDataBusinessDataService(
        IBpmBusinessProcessService bpmBusinessProcessService,
        IBpmnConfService bpmnConfService,
        IBpmnConfLfFormdataFieldService lfFormdataFieldService,
        ILFMainFieldService lfMainFieldService,
        IBpmnNodeService bpmnNodeService,
        IBpmVariableService bpmVariableService,
        IBpmVariableMultiplayerService bpmVariableMultiplayerService,
        IBpmVariableMultiplayerPersonnelService bpmVariableMultiplayerPersonnelService,
        IBpmFlowrunEntrustService bpmFlowrunEntrustService,
        IProcessPermissionsRepository processPermissionsRepository,
        IRoleService roleService,
        IBpmnProcessAdminProvider bpmnProcessAdminProvider,
        IUserRepository userRepository,
        IDepartmentRepository departmentRepository,
        IRoleRepository roleRepository,
        IFreeSql freeSql)
    {
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _bpmnConfService = bpmnConfService;
        _lfFormdataFieldService = lfFormdataFieldService;
        _lfMainFieldService = lfMainFieldService;
        _bpmnNodeService = bpmnNodeService;
        _bpmVariableService = bpmVariableService;
        _bpmVariableMultiplayerService = bpmVariableMultiplayerService;
        _bpmVariableMultiplayerPersonnelService = bpmVariableMultiplayerPersonnelService;
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
        _processPermissionsRepository = processPermissionsRepository;
        _roleService = roleService;
        _bpmnProcessAdminProvider = bpmnProcessAdminProvider;
        _userRepository = userRepository;
        _departmentRepository = departmentRepository;
        _roleRepository = roleRepository;
        _freeSql = freeSql;
    }

    // ==================== 列表 ====================

    public BusinessDataListVo ListPage(BusinessDataListPageReq req)
    {
        if (req == null)
        {
            throw new AFBizException("请选择低代码流程或输入流程编号");
        }
        string? formCode = string.IsNullOrWhiteSpace(req.FormCode) ? null : req.FormCode.Trim();
        string? processNumber = string.IsNullOrWhiteSpace(req.ProcessNumber) ? null : req.ProcessNumber.Trim();
        if (formCode == null && processNumber == null)
        {
            throw new AFBizException("请选择低代码流程或输入流程编号");
        }

        // 1. 主查询分页(流程编号优先: 有流程编号时忽略 formCode 过滤)
        PageDto pageDto = req.PageDto ?? PageDto.First();
        int pageNo = pageDto.Page < 1 ? 1 : pageDto.Page;
        int pageSize = pageDto.PageSize < 1 ? 20 : (pageDto.PageSize > MaxPageSize ? MaxPageSize : pageDto.PageSize);

        string tenantId = MultiTenantUtil.GetCurrentTenantId();
        var query = _bpmBusinessProcessService._repository.GetQueryable()
            .Where(a => a.IsLowCodeFlow == 1
                && a.IsDel == 0
                && (string.IsNullOrEmpty(tenantId) || a.TenantId == tenantId));
        if (processNumber != null)
        {
            query = query.Where(a => a.BusinessNumber != null && a.BusinessNumber.Contains(processNumber));
        }
        else
        {
            query = query.Where(a => a.ProcessinessKey == formCode);
        }
        int total = query.Count();
        List<BpmBusinessProcess> records = query
            .OrderByDescending(a => a.CreateTime)
            .Skip((pageNo - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // 2. 列定义来源 formCode: 未传时取第一条记录(最新)的 ProcessinessKey
        if (formCode == null && records.Count > 0)
        {
            formCode = records[0].ProcessinessKey;
        }
        if (formCode == null)
        {
            // 无记录且未传 formCode: 无列定义,直接返回空
            return new BusinessDataListVo
            {
                Columns = new List<BusinessDataListVo.BusinessDataColumnVo>(),
                Rows = new List<Dictionary<string, object?>>(),
                Total = total,
            };
        }

        // 3. 有效流程配置 -> confId(字段配置链路)
        string confTenantId = MultiTenantUtil.GetCurrentTenantId();
        BpmnConf? bpmnConf = _bpmnConfService._repository.FirstOrDefault(a =>
            a.FormCode == formCode && a.EffectiveStatus == 1 && a.IsDel == 0
            && (string.IsNullOrEmpty(confTenantId) || a.TenantId == confTenantId));
        if (bpmnConf == null)
        {
            throw new AFBizException($"未找到低代码流程 {formCode} 的有效配置");
        }
        long confId = bpmnConf.Id;

        // 4. 字段配置(按id升序,列顺序即创建顺序)
        List<BpmnConfLfFormdataField> fieldConfigs = _lfFormdataFieldService._repository.Find(a =>
                a.BpmnConfId == confId && a.IsDel == 0)
            .OrderBy(a => a.Id)
            .ToList();

        // 5. 隐藏字段集合(任意节点 perm=H -> 后端脱敏)
        HashSet<string> hiddenFieldIds = CollectHiddenFieldIds(confId);

        // 5. 批量查竖表字段值
        List<long> mainIds = records
            .Select(a => a.BusinessId)
            .Where(a => !string.IsNullOrEmpty(a) && long.TryParse(a, out _))
            .Select(long.Parse)
            .Distinct()
            .ToList();
        Dictionary<string, List<LFMainField>> mainId2Fields = new();
        if (mainIds.Count > 0)
        {
            List<LFMainField> lfMainFields = _lfMainFieldService._repository.Find(a =>
                    mainIds.Contains(a.MainId) && a.FormCode == formCode && a.IsDel == 0)
                .Where(a => string.IsNullOrEmpty(a.ParentFieldId))
                .ToList();
            mainId2Fields = lfMainFields.GroupBy(a => a.MainId.ToString())
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        // 6. 拼接 rows
        List<Dictionary<string, object?>> rows = new(records.Count);
        foreach (BpmBusinessProcess bp in records)
        {
            var row = new Dictionary<string, object?>
            {
                ["description"] = bp.Description,
                ["version"] = bp.Version,
            };
            List<LFMainField> mainFields = bp.BusinessId != null && mainId2Fields.TryGetValue(bp.BusinessId, out var mf)
                ? mf : new List<LFMainField>();
            var fieldId2Fields = mainFields.GroupBy(a => a.FieldId)
                .ToDictionary(g => g.Key, g => g.ToList());
            foreach (BpmnConfLfFormdataField fieldConfig in fieldConfigs)
            {
                string fieldId = fieldConfig.FieldId ?? string.Empty;
                if (hiddenFieldIds.Contains(fieldId))
                {
                    row[FieldKey(fieldId)] = MaskValue;
                    continue;
                }
                List<LFMainField>? fields = fieldId2Fields.TryGetValue(fieldId, out var fs) ? fs : null;
                row[FieldKey(fieldId)] = BuildFieldValue(fields, fieldConfig);
            }
            // 流程编号在业务数据之后、发起人之前
            row["processNumber"] = bp.BusinessNumber;
            row["processKey"] = bp.ProcessinessKey;
            row["createUser"] = !string.IsNullOrWhiteSpace(bp.UserName) ? bp.UserName : bp.CreateUser;
            row["processState"] = bp.ProcessState;
            row["processStateName"] = FormatProcessState(bp.ProcessState);
            row["createTime"] = bp.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss");
            rows.Add(row);
        }

        // 7. columns
        List<BusinessDataListVo.BusinessDataColumnVo> columns = BuildColumns(fieldConfigs);

        return new BusinessDataListVo
        {
            Columns = columns,
            Rows = rows,
            Total = total,
        };
    }

    private List<BusinessDataListVo.BusinessDataColumnVo> BuildColumns(List<BpmnConfLfFormdataField> fieldConfigs)
    {
        var columns = new List<BusinessDataListVo.BusinessDataColumnVo>
        {
            new() { Key = "description", Label = "流程名称", Fixed = true },
            new() { Key = "version", Label = "流程版本", Fixed = false },
        };
        foreach (BpmnConfLfFormdataField fieldConfig in fieldConfigs)
        {
            columns.Add(new BusinessDataListVo.BusinessDataColumnVo
            {
                Key = FieldKey(fieldConfig.FieldId ?? string.Empty),
                Label = fieldConfig.FieldName,
                Fixed = false,
            });
        }
        // 流程编号在业务数据之后、发起人之前
        columns.Add(new BusinessDataListVo.BusinessDataColumnVo { Key = "processNumber", Label = "流程编号", Fixed = false });
        columns.Add(new BusinessDataListVo.BusinessDataColumnVo { Key = "processStateName", Label = "流程状态", Fixed = false });
        columns.Add(new BusinessDataListVo.BusinessDataColumnVo { Key = "createUser", Label = "发起人", Fixed = false });
        columns.Add(new BusinessDataListVo.BusinessDataColumnVo { Key = "createTime", Label = "发起时间", Fixed = false });
        return columns;
    }

    /// <summary>
    /// 构建字段展示值:单值标量,多值逗号拼接;JSON 提取 name/label
    /// </summary>
    private object? BuildFieldValue(List<LFMainField>? fields, BpmnConfLfFormdataField fieldConfig)
    {
        if (fields == null || fields.Count == 0)
        {
            return null;
        }
        LFFieldTypeEnum? fieldTypeEnum = fieldConfig.FieldType == null
            ? null : LFFieldTypeEnum.GetByType(fieldConfig.FieldType.Value);
        var values = new List<string>(fields.Count);
        foreach (LFMainField field in fields)
        {
            object? v = ParseSingleFieldValue(field, fieldTypeEnum);
            if (v == null)
            {
                continue;
            }
            string formatted = FormatDisplayValue(v);
            if (!string.IsNullOrEmpty(formatted))
            {
                values.Add(formatted);
            }
        }
        return string.Join(",", values);
    }

    private object? ParseSingleFieldValue(LFMainField field, LFFieldTypeEnum? fieldTypeEnum)
    {
        if (fieldTypeEnum == null)
        {
            return field.FieldValue;
        }
        if (fieldTypeEnum == LFFieldTypeEnum.NUMBER)
        {
            return field.FieldValueNumber;
        }
        if (fieldTypeEnum == LFFieldTypeEnum.DATE_TIME)
        {
            return field.FieldValueDt?.ToString("yyyy-MM-dd HH:mm:ss");
        }
        if (fieldTypeEnum == LFFieldTypeEnum.DATE)
        {
            return field.FieldValueDt?.ToString("yyyy-MM-dd");
        }
        if (fieldTypeEnum == LFFieldTypeEnum.TEXT)
        {
            return field.FieldValueText;
        }
        return field.FieldValue;
    }

    /// <summary>
    /// JSON 值提取 name/label 拼接;非 JSON 原样展示
    /// </summary>
    private string FormatDisplayValue(object? value)
    {
        if (value == null)
        {
            return string.Empty;
        }
        string str = value.ToString()?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(str))
        {
            return string.Empty;
        }
        char first = str[0];
        if (first == '{' || first == '[')
        {
            try
            {
                if (first == '{')
                {
                    return ExtractNameFromJsonObject(str);
                }
                using JsonDocument doc = JsonDocument.Parse(str);
                if (doc.RootElement.ValueKind != JsonValueKind.Array)
                {
                    return str;
                }
                var names = new List<string>();
                foreach (JsonElement item in doc.RootElement.EnumerateArray())
                {
                    string name = ExtractNameFromJsonElement(item);
                    if (!string.IsNullOrEmpty(name))
                    {
                        names.Add(name);
                    }
                }
                return string.Join(",", names);
            }
            catch
            {
                return str;
            }
        }
        return str;
    }

    private string ExtractNameFromJsonObject(string json)
    {
        try
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            return ExtractNameFromJsonElement(doc.RootElement);
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ExtractNameFromJsonElement(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return string.Empty;
        }
        foreach (string key in new[] { "name", "label", "text", "value" })
        {
            if (element.TryGetProperty(key, out JsonElement prop) && prop.ValueKind == JsonValueKind.String)
            {
                string v = prop.GetString() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(v))
                {
                    return v;
                }
            }
        }
        return string.Empty;
    }

    // ==================== 字段脱敏 ====================

    /// <summary>
    /// 汇总 form_code 下所有节点配置中 perm=H 的 fieldId 集合
    /// 来源: t_bpmn_node.node_config_json.lowCodeConf.fieldControls
    /// (.NET 版无 t_bpmn_node_lf_formdata_field_control 独立表,统一走 node_config_json)
    /// </summary>
    private HashSet<string> CollectHiddenFieldIds(long confId)
    {
        var hidden = new HashSet<string>();
        List<BpmnNode> nodes = _bpmnNodeService._repository.Find(a => a.ConfId == confId && a.IsDel == 0);
        foreach (BpmnNode node in nodes)
        {
            if (string.IsNullOrWhiteSpace(node.NodeConfigJson))
            {
                continue;
            }
            try
            {
                BpmnNodeConfigJson? configJson = JsonConfUtil.ParseNodeConfig(node.NodeConfigJson);
                if (configJson?.LowCodeConf?.FieldControls == null)
                {
                    continue;
                }
                foreach (LFFieldControlVO fc in configJson.LowCodeConf.FieldControls)
                {
                    if (StringConstants.HIDDEN_FIELD_PERMISSION.Equals(fc.Perm)
                        && !string.IsNullOrWhiteSpace(fc.FieldId))
                    {
                        hidden.Add(fc.FieldId);
                    }
                }
            }
            catch
            {
                // 忽略单个节点解析失败
            }
        }
        return hidden;
    }

    // ==================== 权限校验 ====================

    public bool CheckPermission(string processNumber)
    {
        if (string.IsNullOrWhiteSpace(processNumber))
        {
            return false;
        }
        BpmBusinessProcess? bp = _bpmBusinessProcessService.GetBpmBusinessProcess(processNumber);
        if (bp == null)
        {
            return false;
        }
        string loginUserId = SecurityUtils.GetLogInEmpIdSafe();

        // 1. 发起人
        if (loginUserId == bp.CreateUser)
        {
            return true;
        }
        // 2. 流程管理员
        try
        {
            BaseIdTranStruVo admin = _bpmnProcessAdminProvider.ProvideProcessAdminInfo();
            if (admin != null && loginUserId == admin.Id)
            {
                return true;
            }
        }
        catch
        {
            // 忽略管理员获取失败
        }
        // 3. 参与人(三表关联)
        string tenantId = MultiTenantUtil.GetCurrentTenantId();
        BpmVariable? bpmVariable = _bpmVariableService._repository.FirstOrDefault(a =>
            a.ProcessNum == processNumber && a.IsDel == 0
            && (string.IsNullOrEmpty(tenantId) || a.TenantId == tenantId));
        if (bpmVariable != null)
        {
            List<BpmVariableMultiplayer> multiplayers = _bpmVariableMultiplayerService._repository.Find(a =>
                a.VariableId == bpmVariable.Id && a.IsDel == 0);
            if (multiplayers.Count > 0)
            {
                List<long> multiplayerIds = multiplayers.Select(a => a.Id).ToList();
                List<BpmVariableMultiplayerPersonnel> personnelList = _bpmVariableMultiplayerPersonnelService._repository.Find(a =>
                    multiplayerIds.Contains(a.VariableMultiplayerId) && a.IsDel == 0);
                if (personnelList.Any(a => a.Assignee == loginUserId))
                {
                    return true;
                }
            }
            // 3.2 加签人员(variable_config_json.signUps)
            if (!string.IsNullOrWhiteSpace(bpmVariable.VariableConfigJson))
            {
                try
                {
                    VariableConfigJson? config = JsonConfUtil.ParseVariableConfig(bpmVariable.VariableConfigJson);
                    if (config?.SignUps != null)
                    {
                        foreach (VariableSignUpItem signUp in config.SignUps)
                        {
                            if (signUp.PersonnelByElement == null)
                            {
                                continue;
                            }
                            foreach (var kv in signUp.PersonnelByElement)
                            {
                                if (kv.Value == null)
                                {
                                    continue;
                                }
                                if (kv.Value.Any(p => !string.IsNullOrWhiteSpace(p.Assignee) && p.Assignee == loginUserId))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // 忽略加签配置解析失败
                }
            }
        }
        // 4. 被委托人(bpm_flowrun_entrust, 过滤减签 action_type=3 及 actual 为0/空)
        if (!string.IsNullOrWhiteSpace(bp.ProcInstId))
        {
            List<BpmFlowrunEntrust> entrusts = _bpmFlowrunEntrustService._repository.Find(a => a.RunInfoId == bp.ProcInstId
                && (string.IsNullOrEmpty(tenantId) || a.TenantId == tenantId));
            if (entrusts.Any(e => e.ActionType != 3
                && !string.IsNullOrWhiteSpace(e.Actual)
                && e.Actual.Trim() != "0"
                && e.Actual == loginUserId))
            {
                return true;
            }
        }
        // 5. 权限表(process_key=form_code, permissions_type in (1,3))
        string? formCode = bp.ProcessinessKey;
        if (!string.IsNullOrWhiteSpace(formCode))
        {
            // 5.1 object_type=1 指定用户
            bool userHit = _processPermissionsRepository.Any(a =>
                a.ProcessKey == formCode
                && a.ObjectType == 1
                && a.ObjectId == loginUserId
                && (a.PermissionsType == 1 || a.PermissionsType == 3)
                && a.IsDel == 0);
            if (userHit)
            {
                return true;
            }
            // 5.2 object_type=3 角色 -> 角色下用户
            List<BpmProcessPermissions> rolePerms = _processPermissionsRepository.Find(a =>
                a.ProcessKey == formCode
                && a.ObjectType == 3
                && (a.PermissionsType == 1 || a.PermissionsType == 3)
                && a.IsDel == 0);
            if (rolePerms.Count > 0)
            {
                List<string> roleIds = rolePerms.Select(a => a.ObjectId)
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .Distinct()
                    .ToList();
                if (roleIds.Count > 0)
                {
                    List<BaseIdTranStruVo> roleUsers = _roleService.QueryUserByRoleIds(roleIds);
                    if (roleUsers.Any(u => u.Id == loginUserId))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // ==================== 人员/部门/角色管理 ====================

    /// <summary>
    /// 人员管理分页列表(姓名/手机号模糊搜索,关联部门名称/直属领导姓名/HRBP姓名)
    /// </summary>
    public ResultAndPage<DemoDataUserVo> UserListPage(DemoDataMgmtPageReq req)
    {
        PageDto pageDto = req.PageDto ?? PageDto.First();
        int pageNo = pageDto.Page < 1 ? 1 : pageDto.Page;
        int pageSize = pageDto.PageSize < 1 ? 20 : (pageDto.PageSize > MaxPageSize ? MaxPageSize : pageDto.PageSize);

        string? userName = string.IsNullOrWhiteSpace(req.UserName) ? null : req.UserName.Trim();
        string? mobile = string.IsNullOrWhiteSpace(req.Mobile) ? null : req.Mobile.Trim();

        var query = _userRepository.GetQueryable();
        if (userName != null)
        {
            query = query.Where(a => a.Name != null && a.Name.Contains(userName));
        }
        if (mobile != null)
        {
            query = query.Where(a => a.Mobile != null && a.Mobile.Contains(mobile));
        }
        int total = query.Count();
        List<User> records = query.OrderBy(a => a.Id).Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

        // 批量补部门/直属领导/HRBP 名称(不 join demo 表,内存关联)
        List<long> deptIds = records.Where(a => a.DepartmentId.HasValue).Select(a => a.DepartmentId!.Value).Distinct().ToList();
        List<long> leaderIds = records.Where(a => a.LeaderId.HasValue).Select(a => a.LeaderId!.Value).Distinct().ToList();
        List<long> hrbpIds = records.Where(a => a.HrbpId.HasValue).Select(a => a.HrbpId!.Value).Distinct().ToList();
        Dictionary<long, string> deptNameMap = BuildDeptNameMap(deptIds);
        Dictionary<long, string> userNameMap = BuildUserNameMap(leaderIds.Concat(hrbpIds).Distinct().ToList());

        List<DemoDataUserVo> vos = records.Select(u => new DemoDataUserVo
        {
            Id = u.Id,
            UserName = u.Name,
            Mobile = u.Mobile,
            Email = u.Email,
            DepartmentId = u.DepartmentId,
            DepartmentName = u.DepartmentId.HasValue && deptNameMap.TryGetValue(u.DepartmentId.Value, out var dn) ? dn : null,
            LeaderId = u.LeaderId,
            LeaderName = u.LeaderId.HasValue && userNameMap.TryGetValue(u.LeaderId.Value, out var ln) ? ln : null,
            HrbpId = u.HrbpId,
            HrbpName = u.HrbpId.HasValue && userNameMap.TryGetValue(u.HrbpId.Value, out var hn) ? hn : null,
            IsDel = u.IsDel == true ? 1 : 0,
        }).ToList();

        return PageUtils.GetResultAndPage(vos, PageDto.BuildCountedPage(pageDto, total));
    }

    /// <summary>
    /// 部门管理分页列表(名称模糊搜索,关联上级部门名称/负责人姓名)
    /// </summary>
    public ResultAndPage<DemoDataDepartmentVo> DepartmentListPage(DemoDataMgmtPageReq req)
    {
        PageDto pageDto = req.PageDto ?? PageDto.First();
        int pageNo = pageDto.Page < 1 ? 1 : pageDto.Page;
        int pageSize = pageDto.PageSize < 1 ? 20 : (pageDto.PageSize > MaxPageSize ? MaxPageSize : pageDto.PageSize);

        string? deptName = string.IsNullOrWhiteSpace(req.DeptName) ? null : req.DeptName.Trim();

        var query = _departmentRepository.GetQueryable();
        if (deptName != null)
        {
            query = query.Where(a => a.Name != null && a.Name.Contains(deptName));
        }
        int total = query.Count();
        List<Department> records = query.OrderBy(a => a.Id).Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

        // 批量补上级部门/负责人名称(内存关联)
        List<long> parentIds = records.Where(a => a.ParentId.HasValue).Select(a => (long)a.ParentId!.Value).Distinct().ToList();
        List<long> leaderIds = records.Where(a => a.LeaderId.HasValue).Select(a => a.LeaderId!.Value).Distinct().ToList();
        Dictionary<long, string> parentNameMap = BuildDeptNameMap(parentIds);
        Dictionary<long, string> userNameMap = BuildUserNameMap(leaderIds);

        List<DemoDataDepartmentVo> vos = records.Select(d => new DemoDataDepartmentVo
        {
            Id = d.Id,
            Name = d.Name,
            ShortName = d.ShortName,
            ParentId = d.ParentId,
            ParentName = d.ParentId.HasValue && parentNameMap.TryGetValue(d.ParentId.Value, out var pn) ? pn : null,
            LeaderId = d.LeaderId,
            LeaderName = d.LeaderId.HasValue && userNameMap.TryGetValue(d.LeaderId.Value, out var ln) ? ln : null,
            Level = d.Level,
            Sort = d.Sort,
            IsDel = d.IsDel == true ? 1 : 0,
        }).ToList();

        return PageUtils.GetResultAndPage(vos, PageDto.BuildCountedPage(pageDto, total));
    }

    /// <summary>
    /// 角色管理分页列表(名称模糊搜索,含角色下关联人数)
    /// </summary>
    public ResultAndPage<DemoDataRoleVo> RoleListPage(DemoDataMgmtPageReq req)
    {
        PageDto pageDto = req.PageDto ?? PageDto.First();
        int pageNo = pageDto.Page < 1 ? 1 : pageDto.Page;
        int pageSize = pageDto.PageSize < 1 ? 20 : (pageDto.PageSize > MaxPageSize ? MaxPageSize : pageDto.PageSize);

        string? roleName = string.IsNullOrWhiteSpace(req.RoleName) ? null : req.RoleName.Trim();

        var query = _roleRepository.GetQueryable();
        if (roleName != null)
        {
            query = query.Where(a => a.RoleName != null && a.RoleName.Contains(roleName));
        }
        int total = query.Count();
        List<Role> records = query.OrderBy(a => a.Id).Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

        // 批量统计角色下人员数量
        List<long> roleIds = records.Select(a => a.Id).ToList();
        Dictionary<long, int> userCountMap = new();
        if (roleIds.Count > 0)
        {
            userCountMap = _freeSql.Select<UserRole>()
                .Where(a => a.RoleId != null && roleIds.Contains(a.RoleId ?? 0L))
                .ToList()
                .GroupBy(a => a.RoleId!.Value)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        List<DemoDataRoleVo> vos = records.Select(r => new DemoDataRoleVo
        {
            Id = r.Id,
            RoleName = r.RoleName,
            UserCount = userCountMap.TryGetValue(r.Id, out int cnt) ? cnt : 0,
        }).ToList();

        return PageUtils.GetResultAndPage(vos, PageDto.BuildCountedPage(pageDto, total));
    }

    /// <summary>
    /// 角色详情:角色下人员分页列表(关联部门名称)
    /// </summary>
    public ResultAndPage<DemoDataRoleUserVo> RoleUsers(DemoDataMgmtPageReq req)
    {
        if (req == null || req.RoleId == null)
        {
            throw new AFBizException("请选择角色");
        }
        PageDto pageDto = req.PageDto ?? PageDto.First();
        int pageNo = pageDto.Page < 1 ? 1 : pageDto.Page;
        int pageSize = pageDto.PageSize < 1 ? 20 : (pageDto.PageSize > MaxPageSize ? MaxPageSize : pageDto.PageSize);

        List<UserRole> all = _freeSql.Select<UserRole>().Where(a => a.RoleId == req.RoleId.Value).ToList();
        List<UserRole> ordered = all.OrderBy(a => a.UserId ?? 0L).ToList();
        int total = ordered.Count;
        List<UserRole> pageItems = ordered.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();

        // 批量补用户信息 + 部门名称(内存关联)
        List<long> userIds = pageItems.Where(a => a.UserId.HasValue).Select(a => a.UserId!.Value).Distinct().ToList();
        Dictionary<long, User> userMap = new();
        if (userIds.Count > 0)
        {
            foreach (User u in _userRepository.Find(a => userIds.Contains(a.Id)))
            {
                userMap.TryAdd(u.Id, u);
            }
        }
        List<long> deptIds = userMap.Values.Where(a => a.DepartmentId.HasValue).Select(a => a.DepartmentId!.Value).Distinct().ToList();
        Dictionary<long, string> deptNameMap = BuildDeptNameMap(deptIds);

        List<DemoDataRoleUserVo> vos = pageItems
            .Where(a => a.UserId.HasValue && userMap.TryGetValue(a.UserId.Value, out _))
            .Select(a =>
            {
                User u = userMap[a.UserId!.Value];
                return new DemoDataRoleUserVo
                {
                    Id = u.Id,
                    UserName = u.Name,
                    Mobile = u.Mobile,
                    Email = u.Email,
                    DepartmentName = u.DepartmentId.HasValue && deptNameMap.TryGetValue(u.DepartmentId.Value, out var dn)
                        ? dn : null,
                };
            }).ToList();

        return PageUtils.GetResultAndPage(vos, PageDto.BuildCountedPage(pageDto, total));
    }

    // ==================== 工具 ====================

    private static string FieldKey(string fieldId)
    {
        return "field_" + fieldId;
    }

    private static string FormatProcessState(int processState)
    {
        return ProcessStateEnumExtensions.GetDescByCode(processState);
    }

    /// <summary>
    /// 部门 id 集合 -> 部门名称 映射(空集合返回空字典)
    /// </summary>
    private Dictionary<long, string> BuildDeptNameMap(List<long> deptIds)
    {
        var map = new Dictionary<long, string>();
        if (deptIds.Count == 0)
        {
            return map;
        }
        List<int> ids = deptIds.Select(x => (int)x).ToList();
        foreach (Department d in _departmentRepository.Find(a => ids.Contains(a.Id)))
        {
            map.TryAdd(d.Id, d.Name ?? string.Empty);
        }
        return map;
    }

    /// <summary>
    /// 用户 id 集合 -> 用户姓名 映射(空集合返回空字典)
    /// </summary>
    private Dictionary<long, string> BuildUserNameMap(List<long> userIds)
    {
        var map = new Dictionary<long, string>();
        if (userIds.Count == 0)
        {
            return map;
        }
        foreach (User u in _userRepository.Find(a => userIds.Contains(a.Id)))
        {
            map.TryAdd(u.Id, u.Name ?? string.Empty);
        }
        return map;
    }
}
