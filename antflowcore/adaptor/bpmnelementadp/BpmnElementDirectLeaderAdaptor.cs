using antflowcore.constant.enus;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.bpmnelementadp;

public class BpmnElementDirectLeaderAdaptor : BpmnElementAdaptor
{
    private readonly ILogger<BpmnElementDirectLeaderAdaptor> _logger;

    public BpmnElementDirectLeaderAdaptor(ILogger<BpmnElementDirectLeaderAdaptor> logger)
    {
        _logger = logger;
    }

    protected override BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo, int elementCode, string elementId)
    {
        // Get the assignee and assignee name or initialize with default values
        var bpmnNodeParamsAssigneeVos = paramsVo.AssigneeList ?? new List<BpmnNodeParamsAssigneeVo>();
        string elementName = bpmnNodeParamsAssigneeVos.FirstOrDefault()?.ElementName;
        var assigneeMap = bpmnNodeParamsAssigneeVos
            .Where(o => o.IsDeduplication == 0)
            .ToDictionary(
                assigneeVo => assigneeVo.Assignee,
                assigneeVo => assigneeVo.AssigneeName,
                StringComparer.OrdinalIgnoreCase);
        
        string collectionName = "directLeader";
        string elementCodeStr = $"directLeader{elementCode}";
        if (property.SignType == (int)SignTypeEnum.SIGN_TYPE_SIGN)
        {
            return BpmnElementUtils.GetMultiplayerSignElement(elementId, elementName, elementCodeStr, assigneeMap.Keys.ToList(), assigneeMap);
        }
        else
        {
            return BpmnElementUtils.GetMultiplayerOrSignElement(elementId, elementName, elementCodeStr, assigneeMap.Keys.ToList(), assigneeMap);
        }
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_DIRECT_LEADER);
    }
}