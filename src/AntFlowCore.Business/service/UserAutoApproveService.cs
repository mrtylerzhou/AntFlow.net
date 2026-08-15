using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Business.service;

/// <summary>
/// 用户自动审批设置 业务服务. 对应 Java UserAutoApproveBizServiceImpl.
/// </summary>
public class UserAutoApproveService : IBpmUserAutoApproveService
{
    public IBpmUserAutoApproveRepository _repository { get; }
    private readonly IBpmnConfService _bpmnConfService;
    private readonly IBpmnNodeService _bpmnNodeService;

    public UserAutoApproveService(IBpmUserAutoApproveRepository repository,
        IBpmnConfService bpmnConfService, IBpmnNodeService bpmnNodeService)
    {
        _repository = repository;
        _bpmnConfService = bpmnConfService;
        _bpmnNodeService = bpmnNodeService;
    }

    // ==================== 列表 ====================

    public ResultAndPage<UserAutoApproveVo> ListPage(PageDto pageDto, string ownerUserName, string ownerUserId, string formCode)
    {
        Page<BpmUserAutoApprove> page = PageUtils.GetPageByPageDto<BpmUserAutoApprove>(pageDto);
        string tenantId = MultiTenantUtil.GetCurrentTenantId();
        List<BpmUserAutoApprove> records = _repository.QueryPageList(ownerUserName, ownerUserId, formCode, tenantId, page);

        Dictionary<string, string> formCode2ActiveBpmnCode = new();
        Dictionary<string, BpmnConf> bpmnCode2Conf = new();
        if (records.Count > 0)
        {
            List<string> formCodes = records.Select(a => a.FormCode).Distinct().ToList();
            List<BpmnConf> activeConfs = _bpmnConfService._repository
                .Find(c => formCodes.Contains(c.FormCode) && c.EffectiveStatus == 1);
            foreach (BpmnConf conf in activeConfs)
            {
                formCode2ActiveBpmnCode.TryAdd(conf.FormCode, conf.BpmnCode);
            }
            List<string> bpmnCodes = records.Select(a => a.BpmnCode).Distinct().ToList();
            foreach (string activeCode in formCode2ActiveBpmnCode.Values)
            {
                if (!bpmnCodes.Contains(activeCode)) bpmnCodes.Add(activeCode);
            }
            List<BpmnConf> confs = _bpmnConfService._repository.Find(c => bpmnCodes.Contains(c.BpmnCode));
            foreach (BpmnConf conf in confs)
            {
                bpmnCode2Conf.TryAdd(conf.BpmnCode, conf);
            }
        }
        List<UserAutoApproveVo> vos = records
            .Select(e => ToVo(e, formCode2ActiveBpmnCode, bpmnCode2Conf)).ToList();
        return PageUtils.GetResultAndPage(vos, PageUtils.GetPageDto(page));
    }

    private UserAutoApproveVo ToVo(BpmUserAutoApprove e,
        Dictionary<string, string> formCode2ActiveBpmnCode, Dictionary<string, BpmnConf> bpmnCode2Conf)
    {
        UserAutoApproveVo vo = new UserAutoApproveVo
        {
            Id = e.Id,
            OwnerUserId = e.OwnerUserId,
            OwnerUserName = e.OwnerUserName,
            FormCode = e.FormCode,
            BpmnCode = e.BpmnCode,
            DefaultComment = e.DefaultComment,
            Enabled = e.Enabled,
            CreateTime = e.CreateTime,
        };
        if (!string.IsNullOrEmpty(e.NodeScopeJson))
        {
            vo.NodeScope = JsonConfUtil.ParseObject<List<UserAutoApproveVo.NodeScopeItem>>(e.NodeScopeJson);
        }
        if (!string.IsNullOrEmpty(e.ConditionJson))
        {
            BpmnNodeAutoNodeConfJson conf = JsonConfUtil.ParseObject<BpmnNodeAutoNodeConfJson>(e.ConditionJson);
            if (conf != null)
            {
                vo.ConditionList = conf.ConditionList;
                vo.GroupRelation = conf.GroupRelation;
            }
        }
        formCode2ActiveBpmnCode.TryGetValue(e.FormCode, out string activeBpmnCode);
        vo.Active = activeBpmnCode != null && activeBpmnCode == e.BpmnCode;
        if (bpmnCode2Conf.TryGetValue(e.BpmnCode, out BpmnConf pointed))
        {
            vo.ConfId = pointed.Id;
            vo.BpmnName = pointed.BpmnName;
            vo.FlowType = ResolveFlowType(pointed);
        }
        return vo;
    }

    private int ResolveFlowType(BpmnConf conf)
    {
        if (conf.IsLowCodeFlow == 1) return 2;
        if (conf.IsOutSideProcess == 1) return 3;
        return 1;
    }

    // ==================== 活跃流程下拉 ====================

    public List<UserAutoApproveVo> ActiveConfList()
    {
        List<BpmnConf> confs = _bpmnConfService._repository.Find(c => c.EffectiveStatus == 1);
        return confs.Select(c => new UserAutoApproveVo
        {
            Id = c.Id,
            FormCode = c.FormCode,
            BpmnCode = c.BpmnCode,
            BpmnName = c.BpmnName,
            FlowType = ResolveFlowType(c),
        }).ToList();
    }

    // ==================== 新增/编辑 ====================

    public void Save(UserAutoApproveVo vo)
    {
        if (string.IsNullOrEmpty(vo.FormCode))
        {
            throw new AFBizException("请选择要自动审批的流程");
        }
        BpmnConf activeConf = GetActiveConf(vo.FormCode);
        if (string.IsNullOrEmpty(vo.OwnerUserId))
        {
            vo.OwnerUserId = SecurityUtils.GetLogInEmpIdStr();
            vo.OwnerUserName = SecurityUtils.GetLogInEmpNameSafe();
        }
        ValidateAndRefreshNodeScope(vo.NodeScope, activeConf.Id);

        BpmUserAutoApprove entity = new BpmUserAutoApprove
        {
            OwnerUserId = vo.OwnerUserId,
            OwnerUserName = vo.OwnerUserName,
            FormCode = vo.FormCode,
            BpmnCode = activeConf.BpmnCode,
            NodeScopeJson = SerializeNodeScope(vo.NodeScope),
            ConditionJson = BuildConditionJson(vo, activeConf),
            DefaultComment = vo.DefaultComment,
            Enabled = vo.Enabled ?? 1,
            IsDel = 0,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
            CreateUser = SecurityUtils.GetLogInEmpNameSafe(),
            CreateTime = DateTime.Now,
        };
        _repository.Add(entity);
    }

    public void Update(UserAutoApproveVo vo)
    {
        if (vo.Id == null)
        {
            throw new AFBizException("id不能为空");
        }
        BpmUserAutoApprove entity = _repository.FirstOrDefault(a => a.Id == vo.Id.Value);
        if (entity == null || entity.IsDel == 1)
        {
            throw new AFBizException("配置不存在");
        }
        BpmnConf pointedConf = _bpmnConfService._repository.FirstOrDefault(c => c.BpmnCode == entity.BpmnCode);
        if (pointedConf == null)
        {
            throw new AFBizException("配置指向的流程版本不存在:" + entity.BpmnCode);
        }
        ValidateAndRefreshNodeScope(vo.NodeScope, pointedConf.Id);
        entity.NodeScopeJson = SerializeNodeScope(vo.NodeScope);
        entity.ConditionJson = BuildConditionJson(vo, pointedConf);
        entity.DefaultComment = vo.DefaultComment;
        if (vo.Enabled != null)
        {
            entity.Enabled = vo.Enabled.Value;
        }
        entity.UpdateUser = SecurityUtils.GetLogInEmpNameSafe();
        entity.UpdateTime = DateTime.Now;
        _repository.Update(entity);
    }

    public void Toggle(long id, int enabled)
    {
        BpmUserAutoApprove entity = _repository.FirstOrDefault(a => a.Id == id);
        if (entity == null || entity.IsDel == 1)
        {
            throw new AFBizException("配置不存在");
        }
        entity.Enabled = enabled;
        entity.UpdateUser = SecurityUtils.GetLogInEmpNameSafe();
        entity.UpdateTime = DateTime.Now;
        _repository.Update(entity);
    }

    public void Delete(long id)
    {
        BpmUserAutoApprove entity = _repository.FirstOrDefault(a => a.Id == id);
        if (entity == null || entity.IsDel == 1)
        {
            throw new AFBizException("配置不存在");
        }
        entity.IsDel = 1;
        entity.UpdateUser = SecurityUtils.GetLogInEmpNameSafe();
        entity.UpdateTime = DateTime.Now;
        _repository.Update(entity);
    }

    // ==================== 复制 ====================

    public void Copy(long id)
    {
        BpmUserAutoApprove config = _repository.FirstOrDefault(a => a.Id == id);
        if (config == null || config.IsDel == 1)
        {
            throw new AFBizException("配置不存在");
        }
        BpmnConf activeConf = GetActiveConf(config.FormCode);
        //同归属人已存在指向活跃版本的配置 → 禁止复制
        bool hasActive = _repository.Any(a => a.OwnerUserId == config.OwnerUserId
            && a.FormCode == config.FormCode
            && a.BpmnCode == activeConf.BpmnCode
            && a.IsDel == 0);
        if (hasActive)
        {
            throw new AFBizException("该流程已存在活跃版本的自动审批配置,不允许复制");
        }
        BpmnConf oldConf = _bpmnConfService._repository.FirstOrDefault(c => c.BpmnCode == config.BpmnCode);
        if (oldConf == null)
        {
            throw new AFBizException("配置指向的旧版本不存在:" + config.BpmnCode);
        }
        //节点对比: 人审节点按elementId对应, 数量+名称一致
        Dictionary<string, string> oldNodes = ApproverNodeMap(oldConf.Id);
        Dictionary<string, string> newNodes = ApproverNodeMap(activeConf.Id);
        if (oldNodes.Count != newNodes.Count)
        {
            throw new AFBizException($"节点数量发生变化(旧:{oldNodes.Count},新:{newNodes.Count}),不允许复制");
        }
        foreach (KeyValuePair<string, string> en in oldNodes)
        {
            if (!newNodes.TryGetValue(en.Key, out string newName) || newName != en.Value)
            {
                throw new AFBizException($"节点[{en.Value}]在最新版本中不存在或名称已变化,不允许复制");
            }
        }
        //表单字段名并集对比
        HashSet<string> oldFields = FormFieldUnion(oldConf.Id);
        HashSet<string> newFields = FormFieldUnion(activeConf.Id);
        if ((oldFields.Count > 0 || newFields.Count > 0) && !oldFields.SetEquals(newFields))
        {
            throw new AFBizException("表单字段发生变化,不允许复制");
        }
        BpmUserAutoApprove copied = new BpmUserAutoApprove
        {
            OwnerUserId = config.OwnerUserId,
            OwnerUserName = config.OwnerUserName,
            FormCode = config.FormCode,
            BpmnCode = activeConf.BpmnCode,
            NodeScopeJson = config.NodeScopeJson,
            ConditionJson = config.ConditionJson,
            DefaultComment = config.DefaultComment,
            Enabled = 1,
            IsDel = 0,
            TenantId = config.TenantId,
            CreateUser = SecurityUtils.GetLogInEmpNameSafe(),
            CreateTime = DateTime.Now,
        };
        _repository.Add(copied);
    }

    // ==================== 运行时查询 ====================

    public List<BpmUserAutoApprove> ListForRuntime(string ownerUserId, string formCode, string bpmnCode)
    {
        return _repository.Find(a => a.OwnerUserId == ownerUserId
            && a.FormCode == formCode
            && a.BpmnCode == bpmnCode
            && a.Enabled == 1
            && a.IsDel == 0);
    }

    // ==================== 内部工具 ====================

    private BpmnConf GetActiveConf(string formCode)
    {
        BpmnConf conf = _bpmnConfService._repository
            .FirstOrDefault(c => c.FormCode == formCode && c.EffectiveStatus == 1);
        if (conf == null)
        {
            throw new AFBizException($"formCode[{formCode}]无活跃的流程版本");
        }
        return conf;
    }

    /// <summary>
    /// 校验节点范围: elementId 必须存在于指定conf的人审节点中, 并以库中名称刷新快照
    /// </summary>
    private void ValidateAndRefreshNodeScope(List<UserAutoApproveVo.NodeScopeItem> nodeScope, long confId)
    {
        if (nodeScope == null || nodeScope.Count == 0)
        {
            return;
        }
        Dictionary<string, string> nodeMap = ApproverNodeMap(confId);
        foreach (UserAutoApproveVo.NodeScopeItem item in nodeScope)
        {
            if (!nodeMap.TryGetValue(item.ElementId, out string name))
            {
                throw new AFBizException($"节点[{item.NodeName}({item.ElementId})]不存在于该流程版本的人审节点中");
            }
            item.NodeName = name;
        }
    }

    private Dictionary<string, string> ApproverNodeMap(long confId)
    {
        List<BpmnNode> nodes = _bpmnNodeService._repository
            .Find(n => n.ConfId == confId && n.NodeType == 4 && n.IsDel == 0);
        Dictionary<string, string> map = new();
        foreach (BpmnNode node in nodes)
        {
            map.TryAdd(node.NodeId, node.NodeName);
        }
        return map;
    }

    /// <summary>
    /// 取conf内各节点表单权限字段名并集
    /// </summary>
    private HashSet<string> FormFieldUnion(long confId)
    {
        List<BpmnNode> nodes = _bpmnNodeService._repository.Find(n => n.ConfId == confId && n.IsDel == 0);
        HashSet<string> fields = new();
        foreach (BpmnNode node in nodes)
        {
            if (string.IsNullOrEmpty(node.NodeConfigJson))
            {
                continue;
            }
            BpmnNodeConfigJson configJson = JsonConfUtil.ParseNodeConfig(node.NodeConfigJson);
            if (configJson?.LowCodeConf?.FieldControls == null || configJson.LowCodeConf.FieldControls.Count == 0)
            {
                continue;
            }
            foreach (var fc in configJson.LowCodeConf.FieldControls)
            {
                if (!string.IsNullOrEmpty(fc.FieldName))
                {
                    fields.Add(fc.FieldName);
                }
            }
        }
        return fields;
    }

    private string SerializeNodeScope(List<UserAutoApproveVo.NodeScopeItem> nodeScope)
    {
        if (nodeScope == null || nodeScope.Count == 0)
        {
            return null;
        }
        return JsonConfUtil.ToJsonString(nodeScope);
    }

    /// <summary>
    /// 条件JSON仅LF流程存储
    /// </summary>
    private string BuildConditionJson(UserAutoApproveVo vo, BpmnConf conf)
    {
        if (conf.IsLowCodeFlow != 1 || vo.ConditionList == null || vo.ConditionList.Count == 0)
        {
            return null;
        }
        BpmnNodeAutoNodeConfJson confJson = new BpmnNodeAutoNodeConfJson
        {
            ConditionList = vo.ConditionList,
            GroupRelation = vo.GroupRelation,
        };
        return JsonConfUtil.ToJsonString(confJson);
    }
}
