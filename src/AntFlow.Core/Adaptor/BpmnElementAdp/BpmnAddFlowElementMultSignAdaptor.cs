using AntFlow.Core.Entity;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.BpmnElementAdp;

public class BpmnAddFlowElementMultSignAdaptor : IBpmnAddFlowElementAdaptor
{
    public void AddFlowElement(BpmnConfCommonElementVo elementVo, AFProcess process,
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