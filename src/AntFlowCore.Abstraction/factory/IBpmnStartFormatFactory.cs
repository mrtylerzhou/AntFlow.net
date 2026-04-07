using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Engine.Engine.factory;

public interface IBpmnStartFormatFactory
{
    void formatBpmnConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}
