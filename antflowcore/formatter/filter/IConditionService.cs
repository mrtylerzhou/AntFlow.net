using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.processor.filter;

public interface IConditionService
{
    bool CheckMatchCondition(BpmnNodeVo nodeVo, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo,bool isDynamicConditionGateway);
}