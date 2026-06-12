using System.Text.Json;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.bpmnnodeadp;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

/// <summary>
/// NodePropertyLevelAdp Class for Level Node Properties
/// </summary>
public class NodePropertyLevelAdaptor : AbstractAdditionSignNodeAdaptor
{
    private readonly ILogger<NodePropertyLevelAdaptor> _logger;

    public NodePropertyLevelAdaptor(
        IRoleService roleService,
        ILogger<NodePropertyLevelAdaptor> logger) : base(roleService)
    {
        _logger = logger;
    }

    public override void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        base.FormatToBpmnNodeVo(bpmnNodeVo);

        // Prefer JSON config if available
        var nodeConfig = bpmnNodeVo.NodeConfigJsonObj;
        if (nodeConfig?.ApproverConf?.AssignLevelConf != null)
        {
            var alc = nodeConfig.ApproverConf.AssignLevelConf;
            bpmnNodeVo.Property = new BpmnNodePropertysVo
            {
                AssignLevelType = alc.AssignLevelType,
                AssignLevelGrade = alc.AssignLevelGrade
            };
            return;
        }

        throw new AFBizException("migration error,please contact the author");
    }

    public PersonnelRuleVo FormaFieldAttributeInfoVO()
    {
        return new PersonnelRuleVo();
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_LEVEL);
    }
}
