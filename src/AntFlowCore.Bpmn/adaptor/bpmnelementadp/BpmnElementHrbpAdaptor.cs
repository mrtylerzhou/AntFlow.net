using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.util;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.vo;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

public class BpmnElementHrbpAdaptor : BpmnElementAdaptor
{
    private readonly ILogger<BpmnElementHrbpAdaptor> _logger;

    public BpmnElementHrbpAdaptor(ILogger<BpmnElementHrbpAdaptor> logger)
    {
        _logger = logger;
    }

    protected override BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo, int elementCode, string elementId)
    {
        // Get the assignee and assignee name or initialize with default values
        var bpmnNodeParamsAssigneeVo = paramsVo.Assignee ?? new BpmnNodeParamsAssigneeVo();
        string assignee = bpmnNodeParamsAssigneeVo.Assignee;
        string assigneeName = bpmnNodeParamsAssigneeVo.AssigneeName;

        var singleAssigneeMap = new Dictionary<string, string>
        {
            { assignee, assigneeName }
        };

        return BpmnElementUtils.GetSingleElement(elementId, bpmnNodeParamsAssigneeVo.ElementName,
            $"hrbpUser{elementCode}", assignee, singleAssigneeMap);
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_HRBP);
    }
}