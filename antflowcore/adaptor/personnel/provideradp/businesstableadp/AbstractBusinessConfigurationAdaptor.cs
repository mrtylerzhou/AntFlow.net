using antflowcore.exception;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.provideradp.businesstableadp;

public abstract class AbstractBusinessConfigurationAdaptor
{
    public abstract List<BpmnNodeParamsAssigneeVo> doFindBusinessPerson(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo bpmnStartConditions);

    protected void ParamValidated(BpmnStartConditionsVo conditionsVo)
    {
        if (conditionsVo == null)
        {
            throw new AFBizException("process has no start conditions");
        }
    }
}