using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.formatter;

public interface IBpmnPersonnelFormat
{
    BpmnConfVo FormatPersonnelsConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}