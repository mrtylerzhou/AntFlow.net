using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.formatter.filter;

public interface IConditionService
{
    bool CheckMatchCondition(BpmnNodeVo nodeVo, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo,bool isDynamicConditionGateway);
}