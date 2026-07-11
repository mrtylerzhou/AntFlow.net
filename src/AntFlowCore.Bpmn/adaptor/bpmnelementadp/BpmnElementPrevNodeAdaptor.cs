using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.util;

namespace AntFlowCore.Bpmn.adaptor.bpmnelementadp;

/// <summary>
/// Element adaptor for previous-node-related user nodes (nodeProperty = 18, NODE_PROPERTY_PREV_NODE_RELATED).
/// Mirrors BpmnElementPersonnelAdaptor but uses collection name "prevNodeUsers".
/// </summary>
public class BpmnElementPrevNodeAdaptor : BpmnElementAdaptor
{
    protected override BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo nodeParamsVo, int elementCode, string elementId)
    {
        var assigneeList = nodeParamsVo.AssigneeList;
        if (assigneeList == null || !assigneeList.Any())
        {
            Console.Error.WriteLine("Assignee list is empty or null.");
            return null;
        }

        string elementName = assigneeList[0].ElementName;

        string collectionName = "prevNodeUsers";

        int? signType = property?.SignType;

        // Build assignee map respecting duplication strategy.
        var startConditions = new BpmnStartConditionsVo();
        object strategyObj = ThreadLocalContainer.Get(StringConstants.DUPLICATION_PROCESS_STRATEGY);
        if (strategyObj is int strategy)
        {
            startConditions.DuplicationProcessStrategy = strategy;
        }
        var assigneeMap = AssigneeVoBuildUtils.DealingWithMultiPlayerNodeDuplication(nodeParamsVo, startConditions);

        string elementCodeStr = string.Join("", collectionName, elementCode);
        if (signType == (int)SignTypeEnum.SIGN_TYPE_SIGN)
        {
            return BpmnElementUtils.GetMultiplayerSignElement(elementId, elementName, elementCodeStr, assigneeMap.Keys.ToList(), assigneeMap);
        }
        else if (signType == (int)SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER)
        {
            return BpmnElementUtils.GetMultiplayerSignInOrderElement(elementId, elementName, elementCodeStr, assigneeMap.Keys.ToList(), assigneeMap);
        }
        else
        {
            return BpmnElementUtils.GetMultiplayerOrSignElement(elementId, elementName, elementCodeStr, assigneeMap.Keys.ToList(), assigneeMap);
        }
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_PREV_NODE_RELATED);
    }
}
