using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.BpmnElementAdp;

public class BpmnAddFlowElementLoopAdaptor : IBpmnAddFlowElementAdaptor
{
    private readonly ILogger<BpmnAddFlowElementLoopAdaptor> _logger;

    public BpmnAddFlowElementLoopAdaptor(ILogger<BpmnAddFlowElementLoopAdaptor> logger)
    {
        _logger = logger;
    }

    public void AddFlowElement(BpmnConfCommonElementVo elementVo, AFProcess process,
        Dictionary<string, object> startParamMap, BpmnStartConditionsVo bpmnStartConditions)
    {
        // 获取元素的代码
        int? elementCode = ProcessNodeEnum.GetCodeByDesc(elementVo.ElementId);

        // 构建任务执行人参数名称和结束条件标记
        string? assigneeParamName = $"loopUser{elementCode}";
        string? endLoopMark = $"${{endLoopMark{elementCode}==true}}";

        // 添加循环任务元素到流程中
        process.AddFlowElement(BpmnBuildUtils.CreateLoopUserTask(
            elementVo.ElementId,
            elementVo.ElementName,
            assigneeParamName,
            endLoopMark
        ));

        // 将默认的结束标记添加到参数映射中
        startParamMap[$"endLoopMark{elementCode}"] = false;

        _logger.LogInformation($"Loop User Task added with ID: {elementVo.ElementId}, Name: {elementVo.ElementName}");
    }
}