using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.formatter;

public interface IBpmnStartFormat
{
    void FormatBpmnConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}