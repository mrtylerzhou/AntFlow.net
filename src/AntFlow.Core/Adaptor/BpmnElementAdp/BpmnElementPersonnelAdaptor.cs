using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.BpmnElementAdp;

public class BpmnElementPersonnelAdaptor : BpmnElementAdaptor
{
    protected override BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo nodeParamsVo,
        int elementCode, string elementId)
    {
        List<BpmnNodeParamsAssigneeVo>? assigneeList = nodeParamsVo.AssigneeList;
        if (assigneeList == null || !assigneeList.Any())
        {
            Console.Error.WriteLine("Assignee list is empty or null.");
            return null; // Or handle the error as needed
        }

        string elementName = assigneeList[0].ElementName;

        string collectionName = "personnelList";

        int? signType = property.SignType;

        SortedDictionary<string, string>? assigneeMap = new();
        List<string>? assignee = new();
        foreach (BpmnNodeParamsAssigneeVo? assigneeVo in assigneeList)
        {
            if (assigneeVo.IsDeduplication == 0)
            {
                assignee.Add(assigneeVo.Assignee);
                assigneeMap[assigneeVo.Assignee] = assigneeVo.AssigneeName;
            }
        }

        string elementCodeStr = string.Join("", collectionName, elementCode);
        if (signType == (int)SignTypeEnum.SIGN_TYPE_SIGN)
        {
            return BpmnElementUtils.GetMultiplayerSignElement(elementId, elementName, elementCodeStr,
                assigneeMap.Keys.ToList(), assigneeMap);
        }

        if (signType == (int)SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER)
        {
            return BpmnElementUtils.GetMultiplayerSignInOrderElement(elementId, elementName, elementCodeStr,
                assigneeMap.Keys.ToList(), assigneeMap);
        }

        return BpmnElementUtils.GetMultiplayerOrSignElement(elementId, elementName, elementCodeStr,
            assigneeMap.Keys.ToList(), assigneeMap);
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_PERSONNEL);
    }
}