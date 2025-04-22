using antflowcore.entity;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.bpmnelementadp;

public class BpmnAddFlowElementSignUpSerialAdaptor : IBpmnAddFlowElementAdaptor
{
    public void AddFlowElement(BpmnConfCommonElementVo elementVo, AFProcess process,
        Dictionary<string, object> startParamMap, BpmnStartConditionsVo bpmnStartConditions)
    {
        // 获取集合值
        var collectionValue = elementVo.CollectionValue;

        // 拼接循环基数的标识
        var loopCardinality = $"{elementVo.ElementId}size";

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