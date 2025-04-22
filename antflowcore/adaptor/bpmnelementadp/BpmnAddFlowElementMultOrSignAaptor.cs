using antflowcore.entity;
using antflowcore.util;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.bpmnelementadp;

public class BpmnAddFlowElementMultOrSignAaptor : IBpmnAddFlowElementAdaptor
{
    private readonly ILogger<BpmnAddFlowElementMultOrSignAaptor> _logger;

    public BpmnAddFlowElementMultOrSignAaptor(ILogger<BpmnAddFlowElementMultOrSignAaptor> logger)
    {
        _logger = logger;
    }

    public void AddFlowElement(BpmnConfCommonElementVo elementVo, AFProcess process,
        Dictionary<string, object> startParamMap, BpmnStartConditionsVo bpmnStartConditions)
    {
        // 添加多用户会签任务
        process.AddFlowElement(BpmnBuildUtils.CreateOrSignUserTask(
            elementVo.ElementId,
            elementVo.ElementName,
            elementVo.CollectionName,
            $"{elementVo.CollectionName.Replace("List", "")}s"));

        // 设置流程启动参数
        startParamMap[elementVo.CollectionName] = elementVo.CollectionValue;

        _logger.LogInformation($"Or Sign User Task added with ID: {elementVo.ElementId}, Name: {elementVo.ElementName}");
    }
}