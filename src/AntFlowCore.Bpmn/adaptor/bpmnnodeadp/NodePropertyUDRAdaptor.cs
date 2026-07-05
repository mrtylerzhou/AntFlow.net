using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.bpmnnodeadp;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

/// <summary>
/// Node adaptor for user-defined-rule (UDR) nodes (nodeProperty = 17, NODE_PROPERTY_ZDY_RULES).
/// Handles the read path: populates BpmnNodePropertysVo.UdrAssigneeProperty, UdrValueJson,
/// SignType and ext fields from the stored JSON config (approverConf.udrConfList) when
/// loading a node for Detail/Preview.
/// </summary>
public class NodePropertyUDRAdaptor : AbstractAdditionSignNodeAdaptor
{
    public NodePropertyUDRAdaptor(IRoleService roleService) : base(roleService)
    {
    }

    public override void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        // Prefer JSON config if available
        var nodeConfig = bpmnNodeVo.NodeConfigJsonObj;
        if (nodeConfig?.ApproverConf?.UdrConfList != null
            && nodeConfig.ApproverConf.UdrConfList.Count > 0)
        {
            var udr = nodeConfig.ApproverConf.UdrConfList[0];

            AfNodeUtils.AddOrEditProperty(bpmnNodeVo, p =>
            {
                p.SignType = udr.SignType;
                p.UdrAssigneeProperty = new BaseIdTranStruVo
                {
                    Id = udr.UdrProperty,
                    Name = udr.UdrPropertyName
                };
                p.UdrValueJson = udr.ValueJson;
                p.Ext1 = udr.Ext1;
                p.Ext2 = udr.Ext2;
                p.Ext3 = udr.Ext3;
                p.Ext4 = udr.Ext4;
            });
            return;
        }

        throw new AFBizException("migration error,please contact the author");
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_UDR_USERS);
    }
}
