using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.Personnel;

public class OutSideOrderedSignNodeAdp : AbstractOrderedSignNodeAdp
{
    public OutSideOrderedSignNodeAdp(AssigneeVoBuildUtils assigneeVoBuildUtils) : base(assigneeVoBuildUtils)
    {
    }

    public override List<string> GetAssigneeIds(BpmnNodeVo nodeVo, BpmnStartConditionsVo bpmnStartConditions)
    {
        string? nodeMark = nodeVo.Property.NodeMark;
        // outside embed node
        List<OutSideBpmAccessEmbedNodeVo>? embedNodes = bpmnStartConditions.EmbedNodes;

        if (string.IsNullOrEmpty(nodeMark) || embedNodes == null || !embedNodes.Any())
        {
            return new List<string> { "0" };
        }

        OutSideBpmAccessEmbedNodeVo? embedNodeVo = embedNodes.FirstOrDefault(o => o.NodeMark == nodeMark);
        if (embedNodeVo == null)
        {
            return new List<string> { "0" };
        }

        List<long>? assigneeIdList = embedNodeVo.AssigneeIdList;
        if (assigneeIdList == null || !assigneeIdList.Any())
        {
            return new List<string> { "0" };
        }

        return AFCollectionUtil.NumberToStringList(assigneeIdList).ToList();
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(OrderNodeTypeEnum.OUT_SIDE_NODE);
    }
}