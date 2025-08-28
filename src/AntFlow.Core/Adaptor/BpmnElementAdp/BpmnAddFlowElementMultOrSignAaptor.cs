using AntFlow.Core.Entity;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.BpmnElementAdp;

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
        // ��Ӷ��û���ǩ����
        process.AddFlowElement(BpmnBuildUtils.CreateOrSignUserTask(
            elementVo.ElementId,
            elementVo.ElementName,
            elementVo.CollectionName,
            $"{elementVo.CollectionName.Replace("List", "")}s"));

        // ����������������
        startParamMap[elementVo.CollectionName] = elementVo.CollectionValue;

        _logger.LogInformation(
            $"Or Sign User Task added with ID: {elementVo.ElementId}, Name: {elementVo.ElementName}");
    }
}