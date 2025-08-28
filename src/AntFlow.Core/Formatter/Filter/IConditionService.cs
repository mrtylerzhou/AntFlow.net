using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Processor.Filter;

public interface IConditionService
{
    bool CheckMatchCondition(BpmnNodeVo nodeVo, BpmnNodeConditionsConfBaseVo conditionsConf,
        BpmnStartConditionsVo bpmnStartConditionsVo, bool isDynamicConditionGateway);
}