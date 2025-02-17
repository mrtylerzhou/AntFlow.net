using antflowcore.conf.di;
using antflowcore.exception;
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
            throw new AFBizException("node can not be null!");
        }

        List<BpmnNodeParamsAssigneeVo> emList = new List<BpmnNodeParamsAssigneeVo>();

        if (bpmnStartConditions.ApproversList != null && bpmnStartConditions.ApproversList.Any())
        {
            // has sign-up approvers
            int fIndex = 1;
            foreach (var s in bpmnStartConditions.ApproversList)
            {
                var vo = new BpmnNodeParamsAssigneeVo
                {
                    Assignee = s,
                    ElementName = string.IsNullOrEmpty(bpmnNodeVo.NodeName) ? $"自定义审批人{fIndex}" : bpmnNodeVo.NodeName
                };
                fIndex++;
                emList.Add(vo);
            }
        }
        else
        {
            // set zero
            var vo = new BpmnNodeParamsAssigneeVo { Assignee = "0" };
            emList.Add(vo);
        }

        return emList;
    }
}