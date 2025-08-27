using AntFlow.Core.Configuration.DependencyInjection;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel.Provider;

[NamedService(nameof(CustomizePersonnelProvider))]
public class CustomizePersonnelProvider : IBpmnPersonnelProviderService
{
    public List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo,
        BpmnStartConditionsVo bpmnStartConditions)
    {
        if (bpmnNodeVo == null)
        {
            throw new System.Exception("node can not be null!");
        }

        Dictionary<string, List<BaseIdTranStruVo>>? nodeId2Assignees = bpmnStartConditions.ApproversList;
        List<string>? currentNodeAssigneeIds = new();
        List<BpmnNodeParamsAssigneeVo>? emList = new();

        if (nodeId2Assignees != null && nodeId2Assignees.Any())
        {
            if (nodeId2Assignees.Count == 1)
            {
                // ֻ��һ���ڵ�ʱ����ǰ�˴���ڵ�id, ���ٸ��ӽ���
                List<string>? ids = nodeId2Assignees.Values
                    .SelectMany(a => a.Select(x => x.Id))
                    .ToList();
                currentNodeAssigneeIds.AddRange(ids);
            }
            else
            {
                if (bpmnNodeVo.Id != null &&
                    nodeId2Assignees.TryGetValue(bpmnNodeVo.Id.ToString(),
                        out List<BaseIdTranStruVo>? baseIdTranStruVos))
                {
                    if (baseIdTranStruVos != null && baseIdTranStruVos.Any())
                    {
                        List<string>? ids = baseIdTranStruVos.Select(x => x.Id).ToList();
                        currentNodeAssigneeIds.AddRange(ids);
                    }
                }
            }

            if (currentNodeAssigneeIds.Any())
            {
                int fIndex = 1;
                foreach (string? s in currentNodeAssigneeIds)
                {
                    BpmnNodeParamsAssigneeVo? vo = new()
                    {
                        Assignee = s,
                        ElementName = !string.IsNullOrEmpty(bpmnNodeVo.NodeName)
                            ? bpmnNodeVo.NodeName
                            : $"�Զ���������{fIndex}"
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