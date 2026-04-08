using AntFlowCore.Core.exception;
using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Core.adaptor.personnel.businesstableadp;

public abstract class AbstractBusinessConfigurationAdaptor
{
    public abstract List<BpmnNodeParamsAssigneeVo> doFindBusinessPerson(BpmnNodeVo bpmnNodeVo, BpmnStartConditionsVo bpmnStartConditions);
    protected void ParamValidated(BpmnStartConditionsVo conditionsVo) {
        if(conditionsVo==null){
            throw new AFBizException("process has no start conditions");
        }

    }
}