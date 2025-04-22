using antflowcore.constant.enums;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.loopsign;

public abstract class AbstractOrderedSignNodeAdp : IAdaptorService
{
    private readonly AssigneeVoBuildUtils _assigneeVoBuildUtils;

    protected AbstractOrderedSignNodeAdp(AssigneeVoBuildUtils assigneeVoBuildUtils)
    {
        _assigneeVoBuildUtils = assigneeVoBuildUtils;
    }

    public abstract List<string> GetAssigneeIds(BpmnNodeVo nodeVo, BpmnStartConditionsVo bpmnStartConditions);

    public void FormatNodes(
        BpmnNodeVo nodeVo,
        BpmnStartConditionsVo bpmnStartConditions,
        string nextNodeId,
        Dictionary<string, BpmnNodeVo> mapPreNodes,
        HashSet<BpmnNodeVo> setAddNodes)
    {
        var orderedSignNodes = GetOrderedSignNodes(nodeVo, bpmnStartConditions, nextNodeId);
        var hasSingleAssignee = nodeVo.Params.AssigneeList != null
                                && nodeVo.Params.AssigneeList.Count == 1
                                && nodeVo.Params.AssigneeList[0].Assignee != "0";

        if (orderedSignNodes.Any() || hasSingleAssignee)
        {
            if (orderedSignNodes.Any())
            {
                mapPreNodes[nextNodeId].Params.NodeTo = orderedSignNodes.Last().NodeId;
                setAddNodes.UnionWith(orderedSignNodes);
            }
            else
            {
                mapPreNodes[nextNodeId].Params.NodeTo = nodeVo.NodeId;
            }
        }
        else
        {
            nodeVo.Params.AssigneeList = new List<BpmnNodeParamsAssigneeVo> { _assigneeVoBuildUtils.BuildZeroVo() };
        }
    }

    private List<BpmnNodeVo> GetOrderedSignNodes(
        BpmnNodeVo nodeVo,
        BpmnStartConditionsVo bpmnStartConditions,
        string nextNodeId)
    {
        if (nodeVo.Params.AssigneeList == null)
        {
            nodeVo.Params.AssigneeList = new List<BpmnNodeParamsAssigneeVo>
                {
                    new BpmnNodeParamsAssigneeVo { Assignee = "0", IsDeduplication = 0 }
                };
        }

        var result = new List<BpmnNodeVo>();

        var listAssign = GetAssignees(nodeVo, bpmnStartConditions);
        if (!listAssign.Any()) return result;

        var emplIds = listAssign.Select(vo => vo.Assignee).ToHashSet();

        if (emplIds.Contains("0") && emplIds.Count == 1)
        {
            return result;
        }

        listAssign = listAssign.Where(a => a.Assignee != "0").ToList();

        var firstAssignee = listAssign.First();
        if (nodeVo.Params.AssigneeList.Count == 1 && nodeVo.Params.AssigneeList[0].Assignee == "0")
        {
            nodeVo.Params.AssigneeList = new List<BpmnNodeParamsAssigneeVo> { firstAssignee };
            nodeVo.Params.ParamType = (int)BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_MULTIPLAYER;
        }

        var nameLast = 1;

        foreach (var bpmnNodeParamsAssigneeVo in listAssign)
        {
            if (bpmnNodeParamsAssigneeVo == firstAssignee) continue;

            var copyNode = SerializationUtils.Clone(nodeVo);
            copyNode.NodeId = $"{nodeVo.NodeId}copy{nameLast}";
            nameLast++;

            copyNode.Property.IsMultiPeople = 1;
            copyNode.Params.AssigneeList = new List<BpmnNodeParamsAssigneeVo> { bpmnNodeParamsAssigneeVo };
            copyNode.Params.ParamType = (int)BpmnNodeParamTypeEnum.BPMN_NODE_PARAM_MULTIPLAYER;
            result.Add(copyNode);
        }

        var forNext = nextNodeId;
        foreach (var vo in result)
        {
            vo.Params.NodeTo = forNext;
            forNext = vo.NodeId;
        }

        return result;
    }

    private List<BpmnNodeParamsAssigneeVo> GetAssignees(
        BpmnNodeVo nodeVo,
        BpmnStartConditionsVo bpmnStartConditions)
    {
        var assigneeIds = GetAssigneeIds(nodeVo, bpmnStartConditions);
        if (assigneeIds == null || !assigneeIds.Any())
        {
            return new List<BpmnNodeParamsAssigneeVo>();
        }

        assigneeIds = assigneeIds.Distinct().ToList();

        var assigneeVos = _assigneeVoBuildUtils.BuildVos(assigneeIds, nodeVo.NodeName, true);
        assigneeVos = assigneeVos.Where(a => a.Assignee != "0").ToList();
        assigneeVos.Add(_assigneeVoBuildUtils.BuildZeroVo());
        return assigneeVos;
    }

    public abstract void SetSupportBusinessObjects();
}