using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.bpmnelementadp;

using antflowcore.constant.enums;
using System.Collections.Generic;

public class BpmnElementStartUserAdaptor : BpmnElementAdaptor
{
    protected override BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo nodeParamsVo, int elementCode, string elementId)
    {
        // Use the assignee from the parameters or create a new object if null
        var bpmnNodeParamsAssigneeVo = nodeParamsVo.Assignee ?? new BpmnNodeParamsAssigneeVo();

        // Extract assignee and assigneeName
        string assignee = bpmnNodeParamsAssigneeVo.Assignee;
        string assigneeName = bpmnNodeParamsAssigneeVo.AssigneeName;

        // Create a map for the assignee
        var singleAssigneeMap = new Dictionary<string, string>
            {
                { assignee, assigneeName }
            };

        // Generate the element with the relevant details
        string elementCodeStr = string.Join("startUser", elementCode);
        return BpmnElementUtils.GetSingleElement(elementId, bpmnNodeParamsAssigneeVo.ElementName, elementCodeStr, assignee, singleAssigneeMap);
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_START_USER);
    }
}