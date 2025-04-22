using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.formatter.filter;

public interface IBpmnOptionalDuplicatesAdaptor
{
    BpmnConfVo OptionalDuplicate(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}