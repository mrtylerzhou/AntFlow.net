using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.BpmnElementAdp;

public class BpmnElementCustomizeAdaptor : BpmnElementAdaptor
{
    private readonly ILogger<BpmnElementCustomizeAdaptor> _logger;

    public BpmnElementCustomizeAdaptor(ILogger<BpmnElementCustomizeAdaptor> logger)
    {
        _logger = logger;
    }

    protected override BpmnConfCommonElementVo GetElementVo(BpmnNodePropertysVo property, BpmnNodeParamsVo paramsVo,
        int elementCode, string elementId)
    {
        List<BpmnNodeParamsAssigneeVo>? assigneeList = paramsVo.AssigneeList;
        string elementName = assigneeList[0].ElementName;

        string collectionName = "customizeUser";
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

        string joinedCollectionName = $"{collectionName}{elementCode}";

        if ((int)SignTypeEnum.SIGN_TYPE_SIGN == signType)
        {
            return BpmnElementUtils.GetMultiplayerSignElement(
                elementId,
                elementName,
                joinedCollectionName,
                assigneeMap.Keys.ToList(),
                assigneeMap
            );
        }

        if ((int)SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER == signType)
        {
            return BpmnElementUtils.GetMultiplayerSignInOrderElement(
                elementId,
                elementName,
                joinedCollectionName,
                assigneeMap.Keys.ToList(),
                assigneeMap
            );
        }

        return BpmnElementUtils.GetMultiplayerOrSignElement(
            elementId,
            elementName,
            joinedCollectionName,
            assigneeMap.Keys.ToList(),
            assigneeMap
        );
    }

    public override void DoFormatNodesToElements(List<BpmnConfCommonElementVo> bpmnConfCommonElementVos,
        BpmnNodeVo nodeVo, int nodeCode, int sequenceFlowNum, Dictionary<string, int> numMap)
    {
        /*BpmnNodeParamsVo paramsVo = nodeVo.Params;
        BpmnNodePropertysVo property = nodeVo.Property??new BpmnNodePropertysVo();
        int signType = property.SignType ?? 1;

        // Filter already deduplicated assignees
        List<BpmnNodeParamsAssigneeVo> paramsAssigneeVos = paramsVo.AssigneeList
            .Where(o => o.IsDeduplication == 0)
            .ToList();

        foreach (var bpmnNodeParamsAssigneeVo in paramsAssigneeVos)
        {
            int elementCodeIncremented = nodeCode + 1;
            string elementId = ProcessNodeEnum.GetDescByCode(elementCodeIncremented);
            int elementSequenceFlowNum = sequenceFlowNum + 1;

            var singleAssigneeMap = new Dictionary<string, string>
            {
                { bpmnNodeParamsAssigneeVo.Assignee, bpmnNodeParamsAssigneeVo.AssigneeName }
            };

            BpmnConfCommonElementVo elementVo = BpmnElementUtils.GetSingleElement(elementId, bpmnNodeParamsAssigneeVo.ElementName,
                $"customizeUser{elementCodeIncremented}", bpmnNodeParamsAssigneeVo.Assignee, singleAssigneeMap);

            // Set node id and buttons
            elementVo.NodeId = nodeVo.Id.ToString();
            elementVo.SignType = signType;
            base.SetElementButtons(nodeVo, elementVo);
            bpmnConfCommonElementVos.Add(elementVo);

            BpmnConfCommonElementVo sequenceFlowElement = BpmnElementUtils.GetSequenceFlow(elementSequenceFlowNum,
                ProcessNodeEnum.GetDescByCode(nodeCode), elementVo.ElementId);

            bpmnConfCommonElementVos.Add(sequenceFlowElement);

            nodeCode++;
            sequenceFlowNum++;

            numMap["nodeCode"] = nodeCode;
            numMap["sequenceFlowNum"] = sequenceFlowNum;
        }*/
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(NodePropertyEnum.NODE_PROPERTY_CUSTOMIZE);
    }
}