using antflowcore.constant.enums;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.bpmnelementadp;

public class BpmnElementBusinessTableAdaptor : BpmnElementAdaptor
{
    /// <summary>
    /// Gets the element VO
    /// </summary>
    /// <param name="property">The node properties</param>
    /// <param name="params">The node parameters</param>
    /// <param name="elementCode">The element code</param>
    /// <param name="elementId">The element ID</param>
    /// <returns>BpmnConfCommonElementVo object</returns>
    protected override BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo @params, int elementCode, string elementId)
    {
        string elementName = @params.AssigneeList[0].ElementName;
        string collectionName = "businessList";
        int? signType = property.SignType;

        // Creating the assignee map
        var assigneeMap = @params.AssigneeList
            .Where(o => o.IsDeduplication == 0)
            .ToDictionary(o => o.Assignee, o => o.AssigneeName);

        if (SignTypeEnum.SIGN_TYPE_SIGN.GetCode() == signType)
        {
            return BpmnElementUtils.GetMultiplayerSignElement(elementId, elementName,
                string.Join("", collectionName, elementCode), assigneeMap.Keys.ToList(), assigneeMap);
        }
        else
        {
            return BpmnElementUtils.GetMultiplayerOrSignElement(elementId, elementName,
                string.Join("", collectionName, elementCode), assigneeMap.Keys.ToList(), assigneeMap);
        }
    }

    /// <summary>
    /// Sets the support for business objects
    /// </summary>
    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_BUSINESSTABLE);
    }
}