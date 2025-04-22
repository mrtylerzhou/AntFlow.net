using antflowcore.conf.di;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.provider;

[NamedService(nameof(CustomizePersonnelProvider))]
public class CustomizePersonnelProvider : IBpmnPersonnelProviderService
{
    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo,
        BpmnStartConditionsVo bpmnStartConditions)
    {
        if (bpmnNodeVo == null)
        {
            throw new Exception("node can not be null!");
        }

        var nodeId2Assignees = bpmnStartConditions.ApproversList;
        var currentNodeAssigneeIds = new List<string>();
        var emList = new List<BpmnNodeParamsAssigneeVo>();

        if (nodeId2Assignees != null && nodeId2Assignees.Any())
        {
            if (nodeId2Assignees.Count == 1)
            {
                // 只有一个节点时忽略前端传入节点id, 减少复杂交互
                var ids = nodeId2Assignees.Values
                    .SelectMany(a => a.Select(x => x.Id))
                    .ToList();
                currentNodeAssigneeIds.AddRange(ids);
            }
            else
            {
                if (bpmnNodeVo.Id != null &&
                    nodeId2Assignees.TryGetValue(bpmnNodeVo.Id.ToString(), out var baseIdTranStruVos))
                {
                    if (baseIdTranStruVos != null && baseIdTranStruVos.Any())
                    {
                        var ids = baseIdTranStruVos.Select(x => x.Id).ToList();
                        currentNodeAssigneeIds.AddRange(ids);
                    }
                }
            }

            if (currentNodeAssigneeIds.Any())
            {
                int fIndex = 1;
                foreach (var s in currentNodeAssigneeIds)
                {
                    var vo = new BpmnNodeParamsAssigneeVo
                    {
                        Assignee = s,
                        ElementName = !string.IsNullOrEmpty(bpmnNodeVo.NodeName)
                            ? bpmnNodeVo.NodeName
                            : $"自定义审批人{fIndex}"
                    };
                    fIndex++;
                    emList.Add(vo);
                }
            }
            else
            {
                emList.Add(new BpmnNodeParamsAssigneeVo { Assignee = "0" });
            }
        }
        else
        {
            emList.Add(new BpmnNodeParamsAssigneeVo { Assignee = "0" });
        }

        return emList;
    }
}