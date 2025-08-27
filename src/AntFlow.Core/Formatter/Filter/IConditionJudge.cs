using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Processor.Filter;

public interface IConditionJudge
{
    bool Judge(string nodeId, BpmnNodeConditionsConfBaseVo conditionsConf, BpmnStartConditionsVo bpmnStartConditionsVo,
        int coundGroup);
}