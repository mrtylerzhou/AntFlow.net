using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.bpmnnodeadp;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Bpmn.adaptor.bpmnnodeadp;

/// <summary>
/// Start user adaptor - directly implements IBpmnNodeAdaptor (not AbstractAdditionSignNodeAdaptor)
/// to match Java version which does not inherit addition sign logic.
/// </summary>
public class NodePropertyStartUserAdaptor : IBpmnNodeAdaptor
{
    public void FormatToBpmnNodeVo(BpmnNodeVo bpmnNodeVo)
    {
        bpmnNodeVo.DeduplicationExclude = true;
    }

    public void EditBpmnNode(BpmnNodeVo bpmnNodeVo)
    {
        // Write path is now handled by BpmnNodeConfigHolder
    }

    public PersonnelRuleVo FormaFieldAttributeInfoVO()
    {
        var vo = new PersonnelRuleVo();
        var nodePropertyStartUser = NodePropertyEnum.NODE_PROPERTY_START_USER;
        vo.NodeProperty = (int)nodePropertyStartUser;
        vo.NodePropertyName = nodePropertyStartUser.GetDesc();
        return vo;
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(BpmnNodeAdpConfEnum.ADP_CONF_NODE_PROPERTY_START_USER);
    }
}
