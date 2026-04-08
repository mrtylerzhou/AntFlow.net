using AntFlowCore.Abstraction.formatter.filter;
using AntFlowCore.Common.util;
using AntFlowCore.Core.exception;
using AntFlowCore.Core.util;
using AntFlowCore.Core.vo;
using AntFlowCore.Extensions.Extensions.service.processor.filter;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor.nodetypecondition.judge;

public class ThirdAccountJudgeService: IConditionJudge
{
    public bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo,int group)
    {
        if (ObjectUtils.IsEmpty(conditionsConf.AccountType)) {
            throw new AFBizException("the process has no third party account conf,please contact the administrator to add one");
        }
        if (bpmnStartConditionsVo.AccountType==null) {
            throw new AFBizException("the process has no account type when start up,but it is a must,please contact the administrator");
        }
        return conditionsConf.AccountType.Contains(bpmnStartConditionsVo.AccountType.Value);
    }
}
