using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Exception;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Processor.Filter;

public class BpmnDeduplicationFormatService : IBpmnDeduplicationFormat
{
    public BpmnConfVo ForwardDeduplication(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions)
    {
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
        BpmnNodeVo bpmnNodeVo = mapNodes[startNodeId];
        List<BpmnNodeVo> nodeVoList = new();

        while (!string.IsNullOrEmpty(bpmnNodeVo.Params.NodeTo))
        {
            bpmnNodeVo = mapNodes[bpmnNodeVo.Params.NodeTo];

            if (bpmnNodeVo.Params.ParamType == 1)
            {
                SinglePlayerNodeDeduplication(bpmnNodeVo, new HashSet<string>(), new List<string> { initiator });
                nodeVoList.Add(bpmnNodeVo);
                continue;
            }

            if (bpmnNodeVo.Params.ParamType == 2)
            {
                MultiPlayerNodeDeduplication(bpmnNodeVo, new HashSet<string>(), new List<string> { initiator }, false);
                nodeVoList.Add(bpmnNodeVo);
            }
        }

        nodeVoList.Reverse();

        List<string> approverList = new();
        foreach (BpmnNodeVo? bpmnNode in nodeVoList)
        {
            if (bpmnNode.Params.ParamType == 1)
            {
                SinglePlayerNodeDeduplication(bpmnNode, new HashSet<string>(), approverList);
                continue;
            }

            if (bpmnNode.Params.ParamType == 2)
            {
                bpmnNode.Params.AssigneeList.Reverse();
                MultiPlayerNodeDeduplication(bpmnNode, new HashSet<string>(), approverList, true);
                bpmnNode.Params.AssigneeList.Reverse();
            }
        }

        return bpmnConfVo;
    }

    public BpmnConfVo BackwardDeduplication(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions)
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

        // 递归处理节点
        ProcessNodeRecursively(bpmnNodeVo, new HashSet<string>(), mapNodes, approverList);

        return bpmnConfVo;
    }

    private void SinglePlayerNodeDeduplication(BpmnNodeVo bpmnNodeVo, HashSet<string> alreadyProcessedNods,
        List<string> approverList)
    {
        if (bpmnNodeVo.Params.IsNodeDeduplication == 1 || alreadyProcessedNods.Contains(bpmnNodeVo.NodeId))
        {
            return;
        }

        BpmnNodeParamsAssigneeVo assignee = bpmnNodeVo.Params.Assignee;
        if (approverList.Contains(assignee.Assignee))
        {
            assignee.IsDeduplication = 1;
            bpmnNodeVo.Params.IsNodeDeduplication = 1;
        }
        else
        {
            approverList.Add(assignee.Assignee);
        }

        alreadyProcessedNods.Add(bpmnNodeVo.NodeId);
    }

    private void MultiPlayerNodeDeduplication(BpmnNodeVo bpmnNodeVo, HashSet<string> alreadyProcessedNods,
        List<string> approverList, bool flag)
    {
        if (bpmnNodeVo.DeduplicationExclude ||
            bpmnNodeVo.Params.IsNodeDeduplication == 1
            || alreadyProcessedNods.Contains(bpmnNodeVo.NodeId))
        {
            return;
        }

        List<BpmnNodeParamsAssigneeVo> assigneeList = bpmnNodeVo.Params.AssigneeList;
        int isNodeDeduplication = 1;
        foreach (BpmnNodeParamsAssigneeVo? assignee in assigneeList)
        {
            if (assignee.IsDeduplication == 1)
            {
                continue;
            }

            if (approverList.Contains(assignee.Assignee))
            {
                assignee.IsDeduplication = 1;
            }
            else
            {
                if (flag)
                {
                    approverList.Add(assignee.Assignee);
                }

                isNodeDeduplication = 0;
            }
        }

        bpmnNodeVo.Params.IsNodeDeduplication = isNodeDeduplication;
        alreadyProcessedNods.Add(bpmnNodeVo.NodeId);
    }

    private void ProcessNodeRecursively(BpmnNodeVo bpmnNodeVo, HashSet<string> alreadyProcessedNodes,
        Dictionary<string, BpmnNodeVo> mapNodes, List<string> approverList)
    {
        string nextId = null;
        do
        {
            if ((int)NodeTypeEnum.NODE_TYPE_PARALLEL_GATEWAY == bpmnNodeVo.NodeType)
            {
                List<string> parallelNodeToIds = bpmnNodeVo.NodeTo;
                foreach (string parallelNodeToId in parallelNodeToIds)
                {
                    if (mapNodes.TryGetValue(parallelNodeToId, out BpmnNodeVo? parallelNodeTo))
                    {
                        ProcessNodeRecursively(parallelNodeTo, alreadyProcessedNodes, mapNodes, approverList);
                    }
                }
            }


            // 处理节点去重
            if (bpmnNodeVo.Params.ParamType == 1)
            {
                SinglePlayerNodeDeduplication(bpmnNodeVo, alreadyProcessedNodes, approverList);
            }
            else if (bpmnNodeVo.Params.ParamType == 2)
            {
                MultiPlayerNodeDeduplication(bpmnNodeVo, alreadyProcessedNodes, approverList, true);
            }

            string nodeTo = GetNodeTo(bpmnNodeVo);

            if (string.IsNullOrEmpty(nodeTo))
            {
                return;
            }

            bpmnNodeVo = GetNextNodeVo(mapNodes.Values, nodeTo);
            nextId = bpmnNodeVo.NodeId;
        } while (!string.IsNullOrEmpty(nextId));
    }

    private BpmnNodeVo GetNextNodeVo(ICollection<BpmnNodeVo> nodes, string nodeTo)
    {
        List<BpmnNodeVo> nextNodeVo = nodes
            .Where(o => o.NodeId == nodeTo)
            .ToList();

        if (nextNodeVo.Count == 0)
        {
            throw new AFBizException("未找到下一个节点");
        }

        return nextNodeVo[0];
    }

    private string GetNodeTo(BpmnNodeVo nodeVo)
    {
        BpmnNodeParamsVo bpmnNodeParamsVo = nodeVo.Params;
        if (bpmnNodeParamsVo == null)
        {
            return null;
        }

        string nodeTo = bpmnNodeParamsVo.NodeTo;
        return nodeTo;
    }
}