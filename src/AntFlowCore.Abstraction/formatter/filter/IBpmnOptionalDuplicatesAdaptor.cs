using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.formatter.filter;

public interface IBpmnOptionalDuplicatesAdaptor
{
    BpmnConfVo OptionalDuplicate(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}