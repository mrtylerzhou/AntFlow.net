using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.formatter.filter;

public interface IBpmnDeduplicationFormat
{
    BpmnConfVo ForwardDeduplication(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);

    BpmnConfVo BackwardDeduplication(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}