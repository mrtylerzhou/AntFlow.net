using AntFlow.Core.Entity;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.BpmnElementAdp;

public interface IBpmnAddFlowElementAdaptor
{
    void AddFlowElement(BpmnConfCommonElementVo elementVo, AFProcess process, Dictionary<string, object> startParamMap,
        BpmnStartConditionsVo bpmnStartConditions);
}