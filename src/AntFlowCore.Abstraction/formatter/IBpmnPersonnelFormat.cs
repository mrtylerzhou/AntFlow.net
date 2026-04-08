using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.formatter;

public interface IBpmnPersonnelFormat
{
    BpmnConfVo FormatPersonnelsConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}