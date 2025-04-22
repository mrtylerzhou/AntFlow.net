using antflowcore.constant.enums;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.formatter.filter;

public class BpmnOptionalDuplicateService : IBpmnOptionalDuplicatesAdaptor
{
    public BpmnConfVo OptionalDuplicate(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions)
    {
        List<string> approverList = new List<string>();
        string startNodeId = null;
        Dictionary<string, BpmnNodeVo> mapNodes = new Dictionary<string, BpmnNodeVo>();
        foreach (var vo in bpmnConfVo.Nodes)
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
            foreach (var assignee in assigneeList)
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