
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.util;
using AntFlowCore.Core;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.vo;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Extensions.Extensions.adaptor.bpmnelementadp;

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