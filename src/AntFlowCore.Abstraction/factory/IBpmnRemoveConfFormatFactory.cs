using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Abstraction.factory;

public interface IBpmnRemoveConfFormatFactory
{
    void RemoveBpmnConf(BpmnConfVo bpmnConfVo, BpmnStartConditionsVo bpmnStartConditions);
}
