using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.processor;

public interface IBpmnStartFormat
{
    void FormatBpmnConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}