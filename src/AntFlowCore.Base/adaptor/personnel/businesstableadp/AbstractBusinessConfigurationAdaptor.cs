using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Base.adaptor.personnel.businesstableadp;

public abstract class AbstractBusinessConfigurationAdaptor
{
    public abstract List<BpmnNodeParamsAssigneeVo> doFindBusinessPerson(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo bpmnStartConditions);
    protected void ParamValidated(BpmnStartConditionsVo conditionsVo) {
        if(conditionsVo==null){
            throw new AFBizException("process has no start conditions");
        }

    }
}