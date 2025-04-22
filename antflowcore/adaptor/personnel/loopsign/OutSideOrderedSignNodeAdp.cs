using antflowcore.constant.enums;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.personnel.loopsign;

public class OutSideOrderedSignNodeAdp : AbstractOrderedSignNodeAdp
{
    public OutSideOrderedSignNodeAdp(AssigneeVoBuildUtils assigneeVoBuildUtils) : base(assigneeVoBuildUtils)
    {
    }

    public override List<string> GetAssigneeIds(BpmnNodeVo nodeVo, BpmnStartConditionsVo bpmnStartConditions)
    {
        var nodeMark = nodeVo.Property.NodeMark;
        // outside embed node
        var embedNodes = bpmnStartConditions.EmbedNodes;

        if (string.IsNullOrEmpty(nodeMark) || embedNodes == null || !embedNodes.Any())
        {
            return new List<string> { "0" };
        }

        var embedNodeVo = embedNodes.FirstOrDefault(o => o.NodeMark == nodeMark);
        if (embedNodeVo == null)
        {
            return new List<string> { "0" };
        }

        var assigneeIdList = embedNodeVo.AssigneeIdList;
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