using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.bpmnelementadp;

using antflowcore.constant.enums;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

public class BpmnElementCustomizeAdaptor : BpmnElementAdaptor
{
    private readonly ILogger<BpmnElementCustomizeAdaptor> _logger;

    public BpmnElementCustomizeAdaptor(ILogger<BpmnElementCustomizeAdaptor> logger)
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

        // Filter already deduplicated assignees
        var paramsAssigneeVos = paramsVo.AssigneeList
            .Where(o => o.IsDeduplication == 0)
            .ToList();

        foreach (var bpmnNodeParamsAssigneeVo in paramsAssigneeVos)
        {
            int elementCodeIncremented = nodeCode + 1;
            string elementId = ProcessNodeEnum.GetDescByCode(elementCodeIncremented);
            int elementSequenceFlowNum = sequenceFlowNum + 1;

            var singleAssigneeMap = new Dictionary<string, string>
                {
                    { bpmnNodeParamsAssigneeVo.Assignee, bpmnNodeParamsAssigneeVo.AssigneeName }
                };

            var elementVo = BpmnElementUtils.GetSingleElement(elementId, bpmnNodeParamsAssigneeVo.ElementName,
                $"customizeUser{elementCodeIncremented}", bpmnNodeParamsAssigneeVo.Assignee, singleAssigneeMap);

            // Set node id and buttons
            elementVo.NodeId = nodeVo.Id.ToString();
            base.SetElementButtons(nodeVo, elementVo);
            bpmnConfCommonElementVos.Add(elementVo);

            var sequenceFlowElement = BpmnElementUtils.GetSequenceFlow(elementSequenceFlowNum,
                ProcessNodeEnum.GetDescByCode(nodeCode), elementVo.ElementId);

            bpmnConfCommonElementVos.Add(sequenceFlowElement);

            nodeCode++;
            sequenceFlowNum++;

            numMap["nodeCode"] = nodeCode;
            numMap["sequenceFlowNum"] = sequenceFlowNum;
        }
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_CUSTOMIZE);
    }
}