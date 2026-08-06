using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Bpmn.adaptor.personnel.loopsign;

public class OutSideOrderedSignNodeAdp : AbstractOrderedSignNodeAdp
{
    public OutSideOrderedSignNodeAdp(AssigneeVoBuildUtils assigneeVoBuildUtils) : base(assigneeVoBuildUtils)
    {
    }

    public override List<List<string>> GetAssigneeIds(BpmnNodeVo nodeVo, BpmnStartConditionsVo bpmnStartConditions)
    {
        var nodeMark = nodeVo.Property.NodeMark;
        // outside embed node
        var embedNodes = bpmnStartConditions.EmbedNodes;

        if (string.IsNullOrEmpty(nodeMark) || embedNodes == null || !embedNodes.Any())
        {
            return new List<List<string>> { new List<string> { "0" } };
        }

        var embedNodeVo = embedNodes.FirstOrDefault(o => o.NodeMark == nodeMark);
        if (embedNodeVo == null)
        {
            return new List<List<string>> { new List<string> { "0" } };
        }

        var assigneeIdList = embedNodeVo.AssigneeIdList;
        if (assigneeIdList == null || !assigneeIdList.Any())
        {
            return new List<List<string>> { new List<string> { "0" } };
        }

        //包法 X:每个 id 独立成一层(每层 1 人),保持链式语义
        var idStrings = AFCollectionUtil.NumberToStringList(assigneeIdList).ToList();
        var result = new List<List<string>>();
        foreach (var id in idStrings)
        {
            result.Add(new List<string> { id });
        }
        return result;
    }

    public override void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(OrderNodeTypeEnum.OUT_SIDE_NODE);
    }
}
