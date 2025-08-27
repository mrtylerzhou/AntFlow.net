using AntFlow.Core.Exception;
using AntFlow.Core.Service.Processor.Filter;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.NodeTypeCondition.Judge;

public class ThirdAccountJudgeService : IConditionJudge
{
    public bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf,
        BpmnStartConditionsVo bpmnStartConditionsVo, int group)
    {
        if (ObjectUtils.IsEmpty(conditionsConf.AccountType))
        {
            throw new AFBizException(
                "the process has no third party account conf,please contact the administrator to add one");
        }

        if (bpmnStartConditionsVo.AccountType == null)
        {
            throw new AFBizException(
                "the process has no account type when start up,but it is a must,please contact the administrator");
        }

        return conditionsConf.AccountType.Contains(bpmnStartConditionsVo.AccountType.Value);
    }
}