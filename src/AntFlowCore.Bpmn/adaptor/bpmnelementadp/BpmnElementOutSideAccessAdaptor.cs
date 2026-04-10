using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.util;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

public class BpmnElementOutSideAccessAdaptor : BpmnElementAdaptor
{
    private readonly ILogger<BpmnElementOutSideAccessAdaptor> _logger;

    public BpmnElementOutSideAccessAdaptor(ILogger<BpmnElementOutSideAccessAdaptor> logger)
    {
        _logger = logger;
    }

    protected override BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo, int elementCode, string elementId)
    {
        var elementName = paramsVo.AssigneeList.FirstOrDefault()?.ElementName;

        string collectionName = "outSideAccessList";

        int? signType = property.SignType;

        // Handle third party element approval assignees
        var assigneeMap = paramsVo.AssigneeList.ToDictionary(
            bpmnNodeParamsAssigneeVo => bpmnNodeParamsAssigneeVo.Assignee,
            bpmnNodeParamsAssigneeVo => bpmnNodeParamsAssigneeVo.AssigneeName
        );

        if (signType == (int)SignTypeEnum.SIGN_TYPE_SIGN)
        {
            return BpmnElementUtils.GetMultiplayerSignElement(
                elementId,
                elementName,
                $"{collectionName}{elementCode}",
                assigneeMap.Keys.ToList(),
                assigneeMap
            );
        }
        else
        {
            return BpmnElementUtils.GetMultiplayerOrSignElement(
                elementId,
                elementName,
                $"{collectionName}{elementCode}",
                assigneeMap.Keys.ToList(),
                assigneeMap
            );
        }
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_OUT_SIDE_ACCESS);
    }
}