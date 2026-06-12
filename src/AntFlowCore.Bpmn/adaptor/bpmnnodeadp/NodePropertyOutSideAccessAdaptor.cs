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
/// NodePropertyOutSideAccessAdp
/// 外部节点访问属性适配器
/// </summary>
public class NodePropertyOutSideAccessAdaptor : IBpmnNodeAdaptor
{
    private readonly ILogger<NodePropertyOutSideAccessAdaptor> _logger;

    public NodePropertyOutSideAccessAdaptor(
        ILogger<NodePropertyOutSideAccessAdaptor> logger)
    {
        _logger = logger;
    }

    public void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        // Prefer JSON config if available
        var nodeConfig = bpmnNodeVo.NodeConfigJsonObj;
        if (nodeConfig?.ApproverConf?.OutSideAccessConf != null)
        {
            var osac = nodeConfig.ApproverConf.OutSideAccessConf;
            bpmnNodeVo.Property = new BpmnNodePropertysVo
            {
                SignType = osac.SignType,
                NodeMark = osac.NodeMark
            };
            bpmnNodeVo.OrderedNodeType = (int)OrderNodeTypeEnum.OUT_SIDE_NODE;
            return;
        }

        throw new AFBizException("migration error,please contact the author");
    }

    public void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
        // Write path is now handled by BpmnNodeConfigHolder
    }

    public PersonnelRuleVo FormaFieldAttributeInfoVO()
    {
        return null;
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_OUT_SIDE_ACCESS);
    }
}
