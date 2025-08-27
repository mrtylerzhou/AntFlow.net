using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Processor.Filter;

public class BpmnOptionalDuplicateService : IBpmnOptionalDuplicatesAdaptor
{
    public BpmnConfVo OptionalDuplicate(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions)
    {
        List<string> approverList = new();
        string startNodeId = null;
        Dictionary<string, BpmnNodeVo> mapNodes = new();
        foreach (BpmnNodeVo? vo in bpmnConfVo.Nodes)
        {
            mapNodes[vo.NodeId] = vo;
            if (vo.NodeType != 0 && vo.NodeType == (int)NodeTypeEnum.NODE_TYPE_START)
            {
                startNodeId = vo.NodeId;
            }
        }

        string initiator = mapNodes[startNodeId].Params.Assignee.Assignee;
        approverList.Add(initiator);
        BpmnNodeVo bpmnNodeVo = mapNodes[startNodeId];

        string optionalNode = "";

        while (string.IsNullOrEmpty(bpmnNodeVo.Params.NodeTo))
        {
            bpmnNodeVo = mapNodes[bpmnNodeVo.Params.NodeTo];

            if (bpmnNodeVo.NodeProperty == 7 && bpmnNodeVo.IsDeduplication == 1)
            {
                optionalNode = bpmnNodeVo.NodeId;
                continue;
            }

            if (bpmnNodeVo.Params.ParamType == 1)
            {
                approverList.Add(bpmnNodeVo.Params.Assignee.Assignee);
                continue;
            }

            if (bpmnNodeVo.Params.ParamType == 2)
            {
                approverList.AddRange(bpmnNodeVo.Params.AssigneeList.Select(a => a.Assignee));
            }
        }

        if (!string.IsNullOrEmpty(optionalNode) && mapNodes.ContainsKey(optionalNode))
        {
            bpmnNodeVo = mapNodes[optionalNode];
            List<BpmnNodeParamsAssigneeVo> assigneeList = bpmnNodeVo.Params.AssigneeList;
            int isNodeDeduplication = 1;
            foreach (BpmnNodeParamsAssigneeVo? assignee in assigneeList)
            {
                if (approverList.Contains(assignee.Assignee))
                {
                    assignee.IsDeduplication = 1;
                }
                else
                {
                    isNodeDeduplication = 0;
                }
            }

            bpmnNodeVo.Params.IsNodeDeduplication = isNodeDeduplication;
        }

        return bpmnConfVo;
    }
}