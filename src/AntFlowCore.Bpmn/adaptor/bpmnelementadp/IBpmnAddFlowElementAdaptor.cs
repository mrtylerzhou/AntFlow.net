using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

public interface IBpmnAddFlowElementAdaptor
{ 
    void AddFlowElement(BpmnConfCommonElementVo elementVo, AFProcess process, Dictionary<String, Object> startParamMap, BpmnStartConditionsVo bpmnStartConditions);

}