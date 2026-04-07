using AntFlowCore.Bpmn.Bpmn.bpmn.build;
using AntFlowCore.Core.entity;
using AntFlowCore.Extensions.Extensions.adaptor.bpmnelementadp;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

public class BpmnAddFlowElementMultSignAdaptor : IBpmnAddFlowElementAdaptor
{
    public  void AddFlowElement(BpmnConfCommonElementVo elementVo, AFProcess process, 
        Dictionary<string, object> startParamMap, BpmnStartConditionsVo bpmnStartConditions)
    {
        // 创建会签任务（Sign User Task）
        process.AddFlowElement(BpmnBuildUtils.CreateSignUserTask(
            elementVo.ElementId,
            elementVo.ElementName,
            elementVo.CollectionName,
            $"{elementVo.CollectionName.Replace("List", "")}s"));

        // 添加启动参数
        startParamMap[elementVo.CollectionName] = elementVo.CollectionValue;
    }
}