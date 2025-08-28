using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Processor.Filter;

public interface IBpmnOptionalDuplicatesAdaptor
{
    BpmnConfVo OptionalDuplicate(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}