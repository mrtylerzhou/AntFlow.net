using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Processor;

public interface IBpmnPersonnelFormat
{
    BpmnConfVo FormatPersonnelsConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}