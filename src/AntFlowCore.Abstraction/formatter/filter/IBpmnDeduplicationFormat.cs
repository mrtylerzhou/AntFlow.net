using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.formatter.filter;

public interface IBpmnDeduplicationFormat
{
   
    BpmnConfVo ForwardDeduplication(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);

  
    BpmnConfVo BackwardDeduplication(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}