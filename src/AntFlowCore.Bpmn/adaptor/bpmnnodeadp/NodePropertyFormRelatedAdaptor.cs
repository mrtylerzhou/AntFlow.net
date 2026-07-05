using System.Text.Json;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.bpmnnodeadp;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

/// <summary>
/// Node adaptor for form-related user nodes (nodeProperty = 16, NODE_PROPERTY_FORM_RELATED).
/// Handles the read path: populates BpmnNodePropertysVo.FormInfos and FormAssigneeProperty
/// from the stored JSON config (approverConf.formRelatedUserConfList) when loading a node for Detail/Preview.
/// </summary>
public class NodePropertyFormRelatedAdaptor : AbstractAdditionSignNodeAdaptor
{
    public NodePropertyFormRelatedAdaptor(IRoleService roleService) : base(roleService)
    {
    }

    public override void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        // Prefer JSON config if available
        var nodeConfig = bpmnNodeVo.NodeConfigJsonObj;
        if (nodeConfig?.ApproverConf?.FormRelatedUserConfList != null
            && nodeConfig.ApproverConf.FormRelatedUserConfList.Count > 0)
        {
            var fr = nodeConfig.ApproverConf.FormRelatedUserConfList[0];
            List<BaseIdTranStruVo> formInfos = new List<BaseIdTranStruVo>();
            if (!string.IsNullOrEmpty(fr.ValueJson))
            {
                formInfos = JsonSerializer.Deserialize<List<BaseIdTranStruVo>>(fr.ValueJson) ?? new List<BaseIdTranStruVo>();
            }

            AfNodeUtils.AddOrEditProperty(bpmnNodeVo, p =>
            {
                p.SignType = fr.SignType;
                p.FormAssigneeProperty = fr.ValueType;
                p.FormInfos = formInfos;
            });
        }
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_FORM_RELATED_USERS);
    }
}
