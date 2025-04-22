using antflowcore.entity;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.bpmnelementadp;

public interface IBpmnAddFlowElementAdaptor
{
    void AddFlowElement(BpmnConfCommonElementVo elementVo, AFProcess process, Dictionary<String, Object> startParamMap, BpmnStartConditionsVo bpmnStartConditions);
}