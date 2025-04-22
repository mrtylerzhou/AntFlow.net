using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.formatter.filter;

public interface IConditionJudge
{
    bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo);
}