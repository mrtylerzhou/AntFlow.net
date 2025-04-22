using antflowcore.exception;
using antflowcore.service.processor.filter;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.nodetypecondition.judge;

public class ThirdAccountJudgeService: IConditionJudge
{
    public bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo)
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
