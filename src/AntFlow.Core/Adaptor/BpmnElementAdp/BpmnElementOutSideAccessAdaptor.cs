using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.BpmnElementAdp;

public class BpmnElementOutSideAccessAdaptor : BpmnElementAdaptor
{
    private readonly ILogger<BpmnElementOutSideAccessAdaptor> _logger;

    public BpmnElementOutSideAccessAdaptor(ILogger<BpmnElementOutSideAccessAdaptor> logger)
    {
        _logger = logger;
    }

    protected override BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo,
        int elementCode, string elementId)
    {
        string? elementName = paramsVo.AssigneeList.FirstOrDefault()?.ElementName;

        string collectionName = "outSideAccessList";

        int? signType = property.SignType;

        // Handle third party element approval assignees
        Dictionary<string, string>? assigneeMap = paramsVo.AssigneeList.ToDictionary(
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

        return BpmnElementUtils.GetMultiplayerOrSignElement(
            elementId,
            elementName,
            $"{collectionName}{elementCode}",
            assigneeMap.Keys.ToList(),
            assigneeMap
        );
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_OUT_SIDE_ACCESS);
    }
}