using AntFlow.Core.Entity;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.BpmnElementAdp;

public class BpmnAddFlowElementSignUpSerialAdaptor : IBpmnAddFlowElementAdaptor
{
    public void AddFlowElement(BpmnConfCommonElementVo elementVo, AFProcess process,
        Dictionary<string, object> startParamMap, BpmnStartConditionsVo bpmnStartConditions)
    {
        // 获取集合值
        List<string>? collectionValue = elementVo.CollectionValue;

        // 拼接循环基数的标识
        string? loopCardinality = $"{elementVo.ElementId}size";

        // 创建顺序循环用户任务并添加到流程
        process.AddFlowElement(BpmnBuildUtils.CreateLoopUserTask(
            elementVo.ElementId,
            elementVo.ElementName,
            loopCardinality,
            elementVo.CollectionName,
            elementVo.CollectionName.Replace("List", "")));

        // 更新流程启动参数
        startParamMap[elementVo.CollectionName] = collectionValue;
        startParamMap[loopCardinality] = collectionValue.Count;
    }
}