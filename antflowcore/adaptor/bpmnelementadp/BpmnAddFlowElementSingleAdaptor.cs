using antflowcore.entity;
using antflowcore.util;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.bpmnelementadp;

public class BpmnAddFlowElementSingleAdaptor : IBpmnAddFlowElementAdaptor
{
    private readonly ILogger<BpmnAddFlowElementSingleAdaptor> _logger;

    public BpmnAddFlowElementSingleAdaptor(ILogger<BpmnAddFlowElementSingleAdaptor> logger)
    {
        _logger = logger;
    }

    public void AddFlowElement(
        BpmnConfCommonElementVo elementVo,
        AFProcess process,
        Dictionary<string, object> startParamMap,
        BpmnStartConditionsVo bpmnStartConditions)
    {
        // 添加用户任务元素
        var userTask = BpmnBuildUtils.CreateUserTask(
            elementVo.ElementId,
            elementVo.ElementName,
            elementVo.AssigneeParamName
        );
        process.AddFlowElement(userTask);

        // 添加到开始参数Map中
        if (!string.IsNullOrEmpty(elementVo.AssigneeParamName))
        {
            startParamMap[elementVo.AssigneeParamName] = elementVo.AssigneeParamValue;
        }

        _logger.LogInformation($"Added user task: {elementVo.ElementId}, Assignee: {elementVo.AssigneeParamName}");
    }
}