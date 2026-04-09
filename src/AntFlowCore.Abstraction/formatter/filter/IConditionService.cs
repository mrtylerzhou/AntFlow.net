using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.formatter.filter;

public interface IConditionService
{
    bool CheckMatchCondition(BpmnNodeVo nodeVo, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo,bool isDynamicConditionGateway);
}