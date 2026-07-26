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

public class NodePropertyCustomizeAdaptor : AbstractAdditionSignNodeAdaptor
{
    private readonly ILogger<NodePropertyCustomizeAdaptor> _logger;

    public NodePropertyCustomizeAdaptor(
        IRoleService roleService,
        ILogger<NodePropertyCustomizeAdaptor> logger) : base(roleService)
    {
        _logger = logger;
    }

    public override void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        base.FormatToBpmnNodeVo(bpmnNodeVo);

        // Prefer JSON config if available
        var nodeConfig = bpmnNodeVo.NodeConfigJsonObj;
        if (nodeConfig?.ApproverConf?.CustomizeConf != null)
        {
            AfNodeUtils.AddOrEditProperty(bpmnNodeVo, p =>
            {
                p.SignType = nodeConfig.ApproverConf.CustomizeConf.SignType;
                p.ArbitrationRatio = nodeConfig.ApproverConf.CustomizeConf.ArbitrationRatio;
            });
            return;
        }

        throw new AFBizException("migration error,please contact the author");
    }

    public PersonnelRuleVo FormaFieldAttributeInfoVO()
    {
        return null;
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_CUSTOMIZE);
    }
}
