using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.vo;
using AntFlowCore.Extensions;

namespace AntFlowCore.Bpmn.adaptor.personnel.loopsign;

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