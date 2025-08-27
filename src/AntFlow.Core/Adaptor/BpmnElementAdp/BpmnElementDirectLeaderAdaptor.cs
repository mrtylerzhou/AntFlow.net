using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.BpmnElementAdp;

public class BpmnElementDirectLeaderAdaptor : BpmnElementAdaptor
{
    private readonly ILogger<BpmnElementDirectLeaderAdaptor> _logger;

    public BpmnElementDirectLeaderAdaptor(ILogger<BpmnElementDirectLeaderAdaptor> logger)
    {
        _logger = logger;
    }

    protected override BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo,
        int elementCode, string elementId)
    {
        // Get the assignee and assignee name or initialize with default values
        BpmnNodeParamsAssigneeVo? bpmnNodeParamsAssigneeVo = paramsVo.Assignee ?? new BpmnNodeParamsAssigneeVo();
        string assignee = bpmnNodeParamsAssigneeVo.Assignee;
        string assigneeName = bpmnNodeParamsAssigneeVo.AssigneeName;

        Dictionary<string, string>? singleAssigneeMap = new() { { assignee, assigneeName } };

        return BpmnElementUtils.GetSingleElement(elementId, bpmnNodeParamsAssigneeVo.ElementName,
            $"directLeader{elementCode}", assignee, singleAssigneeMap);
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_DIRECT_LEADER);
    }
}