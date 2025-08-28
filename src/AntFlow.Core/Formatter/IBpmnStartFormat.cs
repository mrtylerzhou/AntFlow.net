using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Processor;

public interface IBpmnStartFormat
{
    void FormatBpmnConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}