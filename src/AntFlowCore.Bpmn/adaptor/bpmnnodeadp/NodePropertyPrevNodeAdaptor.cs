using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

/// <summary>
/// Node adaptor for previous-node-related user nodes (nodeProperty = 18, NODE_PROPERTY_PREV_NODE_RELATED).
/// Handles the read path: populates BpmnNodePropertysVo.FormAssigneeProperty and SignType
/// from the stored JSON config (approverConf.prevNodeRelatedUserConfList) when loading a node for Detail/Preview.
/// Unlike NodePropertyFormRelatedAdaptor, this does NOT populate FormInfos since previous-node
/// approver does not need form elements, only node property.
/// </summary>
public class NodePropertyPrevNodeAdaptor : AbstractAdditionSignNodeAdaptor
{
    public NodePropertyPrevNodeAdaptor(IRoleService roleService) : base(roleService)
    {
    }

    public override void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        // Prefer JSON config if available
        var nodeConfig = bpmnNodeVo.NodeConfigJsonObj;
        if (nodeConfig?.ApproverConf?.PrevNodeRelatedUserConfList != null
            && nodeConfig.ApproverConf.PrevNodeRelatedUserConfList.Count > 0)
        {
            var conf = nodeConfig.ApproverConf.PrevNodeRelatedUserConfList[0];

            AfNodeUtils.AddOrEditProperty(bpmnNodeVo, p =>
            {
                p.SignType = conf.SignType;
                p.ArbitrationRatio = conf.ArbitrationRatio;
                p.FormAssigneeProperty = conf.ValueType;
            });
        }
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_PREV_NODE_RELATED_USERS);
    }
}
