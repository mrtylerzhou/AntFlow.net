using AntFlowCore.Core.vo;
using AntFlowCore.Vo;

namespace AntFlowCore.Engine.Engine.factory;

public interface IBpmnRemoveConfFormatFactory
{
    void RemoveBpmnConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}
