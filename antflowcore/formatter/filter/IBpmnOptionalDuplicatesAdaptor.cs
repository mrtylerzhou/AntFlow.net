using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.processor.filter;

public interface IBpmnOptionalDuplicatesAdaptor
{
    BpmnConfVo OptionalDuplicate(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}