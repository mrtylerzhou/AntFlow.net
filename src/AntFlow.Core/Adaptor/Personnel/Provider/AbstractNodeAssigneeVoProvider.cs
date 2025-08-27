using AntFlow.Core.Exception;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel.Provider;

public abstract class AbstractNodeAssigneeVoProvider : IBpmnPersonnelProviderService
{
    private readonly AssigneeVoBuildUtils _assigneeVoBuildUtils;

    public AbstractNodeAssigneeVoProvider(AssigneeVoBuildUtils assigneeVoBuildUtils)
    {
        _assigneeVoBuildUtils = assigneeVoBuildUtils;
    }

    public abstract List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo,
        BpmnStartConditionsVo startConditionsVo);

    protected List<BpmnNodeParamsAssigneeVo> ProvideAssigneeList(BpmnNodeVo bpmnNodeVo, ICollection<string> assignees)
    {
        if (bpmnNodeVo.IsOutSideProcess != null && bpmnNodeVo.IsOutSideProcess == 1)
        {
            List<BaseIdTranStruVo>? emplList = bpmnNodeVo.Property.EmplList;
            if (emplList == null || emplList.Count == 0)
            {
                throw new AFBizException("third party process role node has no employee info");
            }

            return _assigneeVoBuildUtils.BuildVOs(emplList, bpmnNodeVo.NodeName, false);
        }

        return _assigneeVoBuildUtils.BuildVos(assignees, bpmnNodeVo.NodeName, false);
    }
}