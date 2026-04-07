using System.Diagnostics;
using AntFlowCore.Core.entity;
using AntFlowCore.Vo;


namespace AntFlowCore.Extensions.Extensions.adaptor.bpmnelementadp;

public interface IBpmnAddFlowElementAdaptor
{ 
    void AddFlowElement(BpmnConfCommonElementVo elementVo, AFProcess process, Dictionary<String, Object> startParamMap, BpmnStartConditionsVo bpmnStartConditions);

}