using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

/// <summary>
/// Adaptor that adds an arbitration sign user task element to the BPMN process.
/// Arbitration sign is a parallel multi-instance task whose completion condition
/// is ${nrOfCompletedInstances >= N} where N = ceil(n * ratio / 100).
/// Mirrors Java BpmnAddFlowElementMultArbitrationAdp.
/// </summary>
public class BpmnAddFlowElementMultArbitrationAdaptor : IBpmnAddFlowElementAdaptor
{
    private readonly ILogger<BpmnAddFlowElementMultArbitrationAdaptor> _logger;

    public BpmnAddFlowElementMultArbitrationAdaptor(ILogger<BpmnAddFlowElementMultArbitrationAdaptor> logger)
    {
        _logger = logger;
    }

    public void AddFlowElement(
        BpmnConfCommonElementVo elementVo,
        AFProcess process,
        Dictionary<string, object> startParamMap,
        BpmnStartConditionsVo bpmnStartConditions)
    {
        process.AddFlowElement(BpmnBuildUtils.CreateArbitrationSignUserTask(elementVo));

        startParamMap[elementVo.CollectionName] = elementVo.CollectionValue;

        _logger.LogInformation(
            "Arbitration Sign User Task added. ElementId={ElementId}, Name={ElementName}, RequiredCount={RequiredCount}, Ratio={Ratio}",
            elementVo.ElementId, elementVo.ElementName, elementVo.RequiredCount, elementVo.ArbitrationRatio);
    }
}