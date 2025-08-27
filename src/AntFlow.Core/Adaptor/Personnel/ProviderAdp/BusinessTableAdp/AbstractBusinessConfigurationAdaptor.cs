using AntFlow.Core.Exception;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel.BusinessTableAdp;

public abstract class AbstractBusinessConfigurationAdaptor
{
    public abstract List<BpmnNodeParamsAssigneeVo> doFindBusinessPerson(BpmnNodeVo bpmnNodeVo,
        BpmnStartConditionsVo bpmnStartConditions);

    protected void ParamValidated(BpmnStartConditionsVo conditionsVo)
    {
        if (conditionsVo == null)
        {
            throw new AFBizException("process has no start conditions");
        }
    }
}