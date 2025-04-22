using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.bpmnelementadp;

using antflowcore.constant.enums;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

public class BpmnElementLoopAdaptor : BpmnElementAdaptor
{
    private readonly ILogger<BpmnElementLoopAdaptor> _logger;

    public BpmnElementLoopAdaptor(ILogger<BpmnElementLoopAdaptor> logger)
    {
        _logger = logger;
    }

    protected override BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo, int elementCode, string elementId)
    {
        return null;
    }

    public new void DoFormatNodesToElements(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos, BpmnNodeVo nodeVo, int nodeCode, int sequenceFlowNum, Dictionary<string, int> numMap)
    {
        var paramsVo = nodeVo.Params;

        // Filter assignees that have already been deduplicated
        var paramsAssigneeVos = paramsVo.AssigneeList
            .Where(o => o.IsDeduplication == 0)
            .ToList();

        // Compatibility with multiple approvals
        if (paramsAssigneeVos.Any() && paramsAssigneeVos.Count > 1 && nodeVo.Property.IsMultiPeople == 1)
        {
            int elementCode = nodeCode + 1;
            string elementId = ProcessNodeEnum.GetDescByCode(elementCode);
            int elementSequenceFlowNum = sequenceFlowNum + 1;

            var assigneeMap = paramsAssigneeVos.ToDictionary(
                bpmnNodeParamsAssigneeVo => bpmnNodeParamsAssigneeVo.Assignee,
                bpmnNodeParamsAssigneeVo => bpmnNodeParamsAssigneeVo.AssigneeName);

            // Set element
            var elementVo = BpmnElementUtils.GetMultiplayerSignElement(
                elementId,
                paramsAssigneeVos[0].ElementName,
                $"loopUserSign{elementCode}",
                assigneeMap.Keys.ToList(),
                assigneeMap
            );

            // Set element buttons
            SetElementButtons(nodeVo, elementVo);

            elementVo.NodeId = nodeVo.Id.ToString();
            elementVo.TemplateVos = nodeVo.TemplateVos;
            elementVo.ApproveRemindVo = nodeVo.ApproveRemindVo;

            bpmnConfCommonElementVos.Add(elementVo);

            // Add flow element
            bpmnConfCommonElementVos.Add(BpmnElementUtils.GetSequenceFlow(elementSequenceFlowNum, ProcessNodeEnum.GetDescByCode(nodeCode), elementVo.ElementId));

            nodeCode++;
            sequenceFlowNum++;

            numMap["nodeCode"] = nodeCode;
            numMap["sequenceFlowNum"] = sequenceFlowNum;

            return;
        }

        // Handle single assignee case
        foreach (var bpmnNodeParamsAssigneeVo in paramsAssigneeVos)
        {
            int elementCode = nodeCode + 1;
            string elementId = ProcessNodeEnum.GetDescByCode(elementCode);
            int elementSequenceFlowNum = sequenceFlowNum + 1;
            string assignee = bpmnNodeParamsAssigneeVo.Assignee;
            string assigneeName = bpmnNodeParamsAssigneeVo.AssigneeName;
            var singleAssigneeMap = new Dictionary<string, string>
                {
                    { assignee, assigneeName }
                };
            var elementVo = BpmnElementUtils.GetSingleElement(
                elementId,
                bpmnNodeParamsAssigneeVo.ElementName,
                $"loopUser{elementCode}",
                assignee,
                singleAssigneeMap
            );

            // Set element buttons
            SetElementButtons(nodeVo, elementVo);

            // Set node message template
            elementVo.TemplateVos = nodeVo.TemplateVos;

            // Set approval reminder
            elementVo.ApproveRemindVo = nodeVo.ApproveRemindVo;

            bpmnConfCommonElementVos.Add(elementVo);

            bpmnConfCommonElementVos.Add(BpmnElementUtils.GetSequenceFlow(elementSequenceFlowNum, ProcessNodeEnum.GetDescByCode(nodeCode), elementVo.ElementId));

            nodeCode++;
            sequenceFlowNum++;

            numMap["nodeCode"] = nodeCode;
            numMap["sequenceFlowNum"] = sequenceFlowNum;
        }
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_LOOP);
    }
}