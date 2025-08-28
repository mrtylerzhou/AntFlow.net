using AntFlow.Core.Bpmn;
using AntFlow.Core.Entity;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.BpmnElementAdp;

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
        // 创建用户任务元素
        UserTask? userTask = BpmnBuildUtils.CreateUserTask(
            elementVo.ElementId,
            elementVo.ElementName,
            elementVo.AssigneeParamName
        );
        process.AddFlowElement(userTask);

        // 添加到启动参数Map中
        if (!string.IsNullOrEmpty(elementVo.AssigneeParamName))
        {
            startParamMap[elementVo.AssigneeParamName] = elementVo.AssigneeParamValue;
        }

        _logger.LogInformation($"Added user task: {elementVo.ElementId}, Assignee: {elementVo.AssigneeParamName}");
    }
}