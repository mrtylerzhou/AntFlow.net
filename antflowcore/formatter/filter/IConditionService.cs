using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.formatter.filter;

public interface IConditionService
{
    bool CheckMatchCondition(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo);
}