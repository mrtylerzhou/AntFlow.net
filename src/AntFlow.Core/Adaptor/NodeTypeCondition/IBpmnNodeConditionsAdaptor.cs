using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.NodeTypeCondition;

public interface IBpmnNodeConditionsAdaptor
{
    void SetConditionsResps(BpmnNodeConditionsConfBaseVo bpmnNodeConditionsConfBaseVo);
}