using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.constants;

namespace AntFlowCore.Bpmn.util;

public static class BpmnElementUtils
{
    private const string SequenceFlowPrefix = "sequenceFlow";
    private const string GateWayPrefix = "gateWay";

    // Get start event element
    public static BpmnConfCommonElementVo GetStartEventElement(string elementId)
    {
        return new BpmnConfCommonElementVo
        {
            ElementId = elementId,
            ElementName = ElementTypeEnum.ELEMENT_TYPE_START_EVENT.Desc,
            ElementType = ElementTypeEnum.ELEMENT_TYPE_START_EVENT.Code
        };
    }

    // Get end event element
    public static BpmnConfCommonElementVo GetEndEventElement(string elementId)
    {
        return new BpmnConfCommonElementVo
        {
            ElementId = elementId,
            ElementName = ElementTypeEnum.ELEMENT_TYPE_END_EVENT.Desc,
            ElementType = ElementTypeEnum.ELEMENT_TYPE_END_EVENT.Code
        };
    }

    // Get single assignee node element
    public static BpmnConfCommonElementVo GetSingleElement(string elementId, string elementName,
        string assigneeParamName, string assigneeParamValue, Dictionary<string, string> assigneeMap)
    {
        return new BpmnConfCommonElementVo
        {
            ElementId = elementId,
            ElementName = elementName,
            ElementType = ElementTypeEnum.ELEMENT_TYPE_USER_TASK.Code,
            ElementProperty = ElementPropertyEnum.ELEMENT_PROPERTY_SINGLE.Code,
            AssigneeParamName = assigneeParamName,
            AssigneeParamValue = assigneeParamValue,
            AssigneeMap = assigneeMap
        };
    }

    // Get multiplayer all-sign node element
    public static BpmnConfCommonElementVo GetMultiplayerSignElement(string elementId, string elementName,
        string collectionName, List<string> collectionValue, IDictionary<string, string> assigneeMap)
    {
        return new BpmnConfCommonElementVo
        {
            ElementId = elementId,
            ElementName = elementName,
            ElementType = ElementTypeEnum.ELEMENT_TYPE_USER_TASK.Code,
            ElementProperty = ElementPropertyEnum.ELEMENT_PROPERTY_MULTIPLAYER_SIGN.Code,
            CollectionName = collectionName,
            CollectionValue = collectionValue,
            AssigneeMap = assigneeMap
        };
    }

    // Get multiplayer all-sign node element (in order)
    public static BpmnConfCommonElementVo GetMultiplayerSignInOrderElement(string elementId, string elementName,
        string collectionName, List<string> collectionValue, IDictionary<string, string> assigneeMap)
    {
        return new BpmnConfCommonElementVo
        {
            ElementId = elementId,
            ElementName = elementName,
            ElementType = ElementTypeEnum.ELEMENT_TYPE_USER_TASK.Code,
            ElementProperty = ElementPropertyEnum.ELEMENT_PROPERTY_MULTIPLAYER_SIGN_IN_ORDER.Code,
            CollectionName = collectionName,
            CollectionValue = collectionValue,
            AssigneeMap = assigneeMap
        };
    }

    // Get multiplayer or-sign node element
    public static BpmnConfCommonElementVo GetMultiplayerOrSignElement(string elementId, string elementName,
        string collectionName, List<string> collectionValue, IDictionary<string, string> assigneeMap)
    {
        return new BpmnConfCommonElementVo
        {
            ElementId = elementId,
            ElementName = elementName,
            ElementType = ElementTypeEnum.ELEMENT_TYPE_USER_TASK.Code,
            ElementProperty = ElementPropertyEnum.ELEMENT_PROPERTY_MULTIPLAYER_ORSIGN.Code,
            CollectionName = collectionName,
            CollectionValue = collectionValue,
            AssigneeMap = assigneeMap
        };
    }

    // Get multiplayer arbitration-sign node element
    // N = ceil(size * ratio / 100), min 1, max size
    public static BpmnConfCommonElementVo GetMultiplayerArbitrationElement(string elementId, string elementName,
        string collectionName, List<string> collectionValue, IDictionary<string, string> assigneeMap, int? ratio)
    {
        if (ratio == null)
        {
            throw new ArgumentException("仲裁签未配置通过比例");
        }
        int size = collectionValue.Count;
        int requiredCount = (int)Math.Ceiling(size * ratio.Value / 100.0);
        requiredCount = Math.Max(1, Math.Min(requiredCount, size));
        return new BpmnConfCommonElementVo
        {
            ElementId = elementId,
            ElementName = elementName,
            ElementType = ElementTypeEnum.ELEMENT_TYPE_USER_TASK.Code,
            ElementProperty = ElementPropertyEnum.ELEMENT_PROPERTY_MULTIPLAYER_ARBITRATION.Code,
            CollectionName = collectionName,
            CollectionValue = collectionValue,
            AssigneeMap = assigneeMap,
            SignType = (int)SignTypeEnum.SIGN_TYPE_ARBITRATION,
            RequiredCount = requiredCount,
            ArbitrationRatio = ratio
        };
    }

    // Get sign-up node element
    public static BpmnConfCommonElementVo GetSignUpElement(string elementId, BpmnConfCommonElementVo fatherElementVo,
        int elementProperty)
    {
        return new BpmnConfCommonElementVo
        {
            ElementId = elementId,
            ElementName = $"{fatherElementVo.ElementName}加批",
            ElementType = ElementTypeEnum.ELEMENT_TYPE_USER_TASK.Code,
            ElementProperty = elementProperty,
            CollectionName = $"{elementId}signUpUserList",
            CollectionValue = new List<string>()
        };
    }

    // Get flow line elements
    public static BpmnConfCommonElementVo GetSequenceFlow(int sequenceFlowNum, string flowFrom, string flowTo)
    {
        return new BpmnConfCommonElementVo
        {
            ElementId = $"{SequenceFlowPrefix}{sequenceFlowNum}",
            ElementName = $"{SequenceFlowPrefix}{sequenceFlowNum}",
            ElementType = ElementTypeEnum.ELEMENT_TYPE_SEQUENCE_FLOW.Code,
            ElementProperty = ElementPropertyEnum.ELEMENT_PROPERTY_SEQUENCE_FLOW.Code,
            FlowFrom = flowFrom,
            FlowTo = flowTo
        };
    }

    // Get parallel gateway element
    public static BpmnConfCommonElementVo GetParallelGateWayElement(int sequenceFlowNum)
    {
        return new BpmnConfCommonElementVo
        {
            ElementId = $"{GateWayPrefix}{sequenceFlowNum}",
            ElementName = $"{GateWayPrefix}{sequenceFlowNum}",
            ElementType = ElementTypeEnum.ELEMENT_TYPE_PARALLEL_GATEWAY.Code,
            ElementProperty = ElementPropertyEnum.ELEMENT_PROPERTY_PARALLEL_GATEWAY.Code
        };
    }
}
