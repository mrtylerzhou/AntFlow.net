using System.Text.Json;
using AntFlowCore.Constant.Enums;
using AntFlowCore.Entity;
using antflowcore.constant.enus;
using antflowcore.exception;
using antflowcore.service.interf.repository;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;

namespace antflowcore.adaptor.bpmnnodeadp;

/// <summary>
/// 表单关联人员审批规则适配器
/// </summary>
public class NodePropertyFormRelatedAdaptor : AbstractCommonBpmnNodeAdaptor<BpmnNodeFormRelatedUserConf>
{
    private readonly BpmnNodeFormRelatedUserConfService _bpmnNodeFormRelatedUserConfService;

    public NodePropertyFormRelatedAdaptor(
        BpmnNodeFormRelatedUserConfService bpmnNodeFormRelatedUserConfService,
        BpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
        IRoleService roleService) : base(bpmnNodeAdditionalSignConfService, roleService)
    {
        _bpmnNodeFormRelatedUserConfService = bpmnNodeFormRelatedUserConfService;
    }

    protected override void SetNodeProperty(BpmnNodeVo nodeVo, BpmnNodeFormRelatedUserConf entity)
    {
        if (entity == null)
        {
            throw new AFBizException("无法从数据库获取配置信息");
        }

        int? nodeProperty = nodeVo.NodeProperty;
        if (nodeProperty != (int)NodePropertyEnum.NODE_PROPERTY_FORM_RELATED)
        {
            throw new AFBizException("PARAMS_MISMATCH", "逻辑错误，请联系管理员");
        }

        var formInfos = string.IsNullOrEmpty(entity.ValueJson)
            ? new List<BaseIdTranStruVo>()
            : JsonSerializer.Deserialize<List<BaseIdTranStruVo>>(entity.ValueJson) ?? new List<BaseIdTranStruVo>();

        AfNodeUtils.AddOrEditProperty(nodeVo, p =>
        {
            p.SignType = entity.SignType;
            p.FormAssigneeProperty = entity.ValueType;
            p.FormInfos = formInfos;
        });
    }

    protected override List<BpmnNodeFormRelatedUserConf> BuildEntity(BpmnNodeVo nodeVo)
    {
        var property = nodeVo.Property;
        var entity = new BpmnNodeFormRelatedUserConf
        {
            BpmnNodeId = nodeVo.Id,
            SignType = property.SignType,
            ValueJson = property.FormInfos == null ? "[]" : JsonSerializer.Serialize(property.FormInfos),
            ValueType = property.FormAssigneeProperty,
            ValueTypeName = NodeFormAssigneeProperty.GetDescByCode(property.FormAssigneeProperty),
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
            CreateUser = SecurityUtils.GetLogInEmpName(),
            CreateTime = DateTime.Now,
            UpdateUser = SecurityUtils.GetLogInEmpName(),
            UpdateTime = DateTime.Now,
            IsDel = 0,
        };

        return new List<BpmnNodeFormRelatedUserConf> { entity };
    }

    protected override void CheckParam(BpmnNodeVo nodeVo)
    {
        var property = nodeVo.Property;
        if (property == null)
        {
            throw new AFBizException("PARAMS_NOT_COMPLETE", "节点扩展参数:property不能为空!");
        }

        if (property.SignType == null)
        {
            property.SignType = 1;
        }

        if (property.FormInfos == null || property.FormInfos.Count == 0)
        {
            throw new AFBizException("PARAMS_NOT_COMPLETE", "节点扩展参数:formInfos不能为空!");
        }

        if (property.FormAssigneeProperty == null)
        {
            throw new AFBizException("PARAMS_NOT_COMPLETE", "节点扩展参数:formAssigneeProperty不能为空!");
        }

        if (NodeFormAssigneeProperty.GetByCode(property.FormAssigneeProperty) == null)
        {
            throw new AFBizException("PARAMS_NULL_AFTER_CONVERT", "节点扩展参数:formAssigneeProperty类型未定义!");
        }
    }

    protected override BpmnNodeFormRelatedUserConf QueryEntity(BpmnNodeVo nodeVo)
    {
        return _bpmnNodeFormRelatedUserConfService.baseRepo
            .Where(x => x.BpmnNodeId == nodeVo.Id && x.IsDel != 1)
            .First();
    }

    protected override void SaveEntities(List<BpmnNodeFormRelatedUserConf> entities)
    {
        _bpmnNodeFormRelatedUserConfService.baseRepo.Insert(entities);
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_FORM_RELATED_USERS);
    }
}
