using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Processor.Filter;

public interface IBpmnDeduplicationFormat
{
    BpmnConfVo ForwardDeduplication(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);


    BpmnConfVo BackwardDeduplication(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}