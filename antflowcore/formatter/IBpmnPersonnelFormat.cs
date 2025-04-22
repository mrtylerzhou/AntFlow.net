using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.service.processor;

public interface IBpmnPersonnelFormat
{
    BpmnConfVo FormatPersonnelsConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}