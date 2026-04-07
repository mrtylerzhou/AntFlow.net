using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.factory;

public interface IBpmnStartFormatFactory
{
    void formatBpmnConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}
