using AntFlowCore.Abstraction.formatter.filter;
using AntFlowCore.Core.vo;
using AntFlowCore.Extensions.Extensions.service.processor.filter;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor.nodetypecondition.judge;

public class NumberOperatorJudgeService: IConditionJudge
{
    public bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo,int group)
    {
        return true;
    }
}