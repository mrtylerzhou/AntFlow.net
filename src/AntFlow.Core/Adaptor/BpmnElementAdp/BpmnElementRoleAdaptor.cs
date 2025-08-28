using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.BpmnElementAdp;

public class BpmnElementRoleAdaptor : BpmnElementAdaptor
{
    protected override BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo nodeParamsVo,
        int elementCode, string elementId)
    {
        // Get the element name from the first assignee
        string elementName = nodeParamsVo.AssigneeList.FirstOrDefault()?.ElementName;

        // Create the assignee map while ensuring there is no duplication
        Dictionary<string, string>? assigneeMap = nodeParamsVo.AssigneeList
            .Where(o => o.IsDeduplication == 0)
            .ToDictionary(
                assigneeVo => assigneeVo.Assignee,
                assigneeVo => assigneeVo.AssigneeName,
                StringComparer.OrdinalIgnoreCase);

        // Determine the sign type and call the appropriate method
        string collectionName = "roleUserList";
        string elementCodeStr = string.Join(collectionName, elementCode);

        if (property.SignType == (int)SignTypeEnum.SIGN_TYPE_SIGN)
        {
            return BpmnElementUtils.GetMultiplayerSignElement(elementId, elementName, elementCodeStr,
                assigneeMap.Keys.ToList(), assigneeMap);
        }

        return BpmnElementUtils.GetMultiplayerOrSignElement(elementId, elementName, elementCodeStr,
            assigneeMap.Keys.ToList(), assigneeMap);
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_ROLE);
    }
}