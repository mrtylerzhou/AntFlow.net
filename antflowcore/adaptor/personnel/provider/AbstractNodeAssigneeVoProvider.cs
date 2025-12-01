using antflowcore.exception;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.provider;

public abstract class AbstractNodeAssigneeVoProvider : IBpmnPersonnelProviderService
{
    private readonly AssigneeVoBuildUtils _assigneeVoBuildUtils;

    public AbstractNodeAssigneeVoProvider(AssigneeVoBuildUtils assigneeVoBuildUtils)
    {
        _assigneeVoBuildUtils = assigneeVoBuildUtils;
    }

    protected virtual List<BpmnNodeParamsAssigneeVo> ProvideAssigneeList(BpmnNodeVo bpmnNodeVo, ICollection<BaseIdTranStruVo>  assignees)
    {
        if (bpmnNodeVo.IsOutSideProcess != null && bpmnNodeVo.IsOutSideProcess == 1)
        {
            var emplList = bpmnNodeVo.Property.EmplList;
            if (emplList == null || emplList.Count == 0)
            {
                throw new AFBizException("third party process role node has no employee info");
            }

            return _assigneeVoBuildUtils.BuildVOs(emplList, bpmnNodeVo.NodeName, false);
        }
        return _assigneeVoBuildUtils.BuildVOs(assignees, bpmnNodeVo.NodeName, false);
    }

    public abstract List<BpmnNodeParamsAssigneeVo> GetAssigneeList(BpmnNodeVo bpmnNodeVo,
        BpmnStartConditionsVo startConditionsVo);

}