using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.processor.filter;

public interface IConditionService
{
    bool CheckMatchCondition(String nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo);
}