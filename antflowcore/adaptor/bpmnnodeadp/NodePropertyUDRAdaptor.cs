using AntFlowCore.Entity;
using antflowcore.constant.enus;
using antflowcore.exception;
using antflowcore.service.interf.repository;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;

namespace antflowcore.adaptor.bpmnnodeadp;

/// <summary>
/// 用户自定义规则节点属性适配器
/// 继承AbstractCommonBpmnNodeAdaptor实现数据持久化
/// </summary>
public class NodePropertyUDRAdaptor : AbstractCommonBpmnNodeAdaptor<BpmnNodeUDRConf>
{
    private readonly BpmnNodeUDRConfService _bpmnNodeUDRConfService;

    public NodePropertyUDRAdaptor(
        BpmnNodeUDRConfService bpmnNodeUDRConfService,
        BpmnNodeAdditionalSignConfService bpmnNodeAdditionalSignConfService,
        IRoleService roleService) : base(bpmnNodeAdditionalSignConfService, roleService)
    {
        _bpmnNodeUDRConfService = bpmnNodeUDRConfService;
    }

    protected override void SetNodeProperty(BpmnNodeVo bpmnNodeVo, BpmnNodeUDRConf entity)
    {
        if (entity == null)
        {
            throw new AFBizException("无法从数据库获取配置信息");
        }

        int? nodeProperty = bpmnNodeVo.NodeProperty;
        if (nodeProperty != (int)NodePropertyEnum.NODE_PROPERTY_ZDY_RULES)
        {
            throw new AFBizException("PARAMS_MISMATCH", "逻辑错误，请联系管理员");
        }

        AfNodeUtils.AddOrEditProperty(bpmnNodeVo, p =>
        {
            p.SignType = entity.SignType;
            p.UdrAssigneeProperty = new BaseIdTranStruVo { Id = entity.UdrProperty, Name = entity.UdrPropertyName };
            p.UdrValueJson = entity.ValueJson;
        });
    }

    protected override List<BpmnNodeUDRConf> BuildEntity(BpmnNodeVo nodeVo)
    {
        var property = nodeVo.Property;
        var entity = new BpmnNodeUDRConf
        {
            BpmnNodeId = nodeVo.Id,
            SignType = property.SignType,
            UdrProperty = property.UdrAssigneeProperty?.Id,
            UdrPropertyName = property.UdrAssigneeProperty?.Name,
            ValueJson = property.UdrValueJson,
            Ext1 = property.Ext1,
            Ext2 = property.Ext2,
            Ext3 = property.Ext3,
            Ext4 = property.Ext4,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
            CreateUser = SecurityUtils.GetLogInEmpName(),
            CreateTime = DateTime.Now,
            UpdateUser = SecurityUtils.GetLogInEmpName(),
            UpdateTime = DateTime.Now,
            IsDel = 0,
        };

        return new List<BpmnNodeUDRConf> { entity };
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

        if (property.UdrAssigneeProperty == null)
        {
            throw new AFBizException("PARAMS_NOT_COMPLETE", "节点扩展参数:udrAssigneeProperty不能为空!");
        }
    }

    protected override BpmnNodeUDRConf QueryEntity(BpmnNodeVo nodeVo)
    {
        return _bpmnNodeUDRConfService.baseRepo
            .Where(x => x.BpmnNodeId == nodeVo.Id && x.IsDel != 1)
            .First();
    }

    protected override void SaveEntities(List<BpmnNodeUDRConf> entities)
    {
        _bpmnNodeUDRConfService.baseRepo.Insert(entities);
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_UDR_USERS);
    }
}
