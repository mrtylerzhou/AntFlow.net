using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;

namespace AntFlowCore.Base.util;

public class AfNodeUtils
{
    public static void AddOrEditProperty(BpmnNodeVo bpmnNodeVo, Action<BpmnNodePropertysVo> action)
    {
        if (bpmnNodeVo.Property == null)
        {
            bpmnNodeVo.Property = new BpmnNodePropertysVo();
        }

        action(bpmnNodeVo.Property);
    }

    /// <summary>
    /// 设计时保存流程模板前的特殊节点处理.
    /// 将抄送节点v2(nodeType=8)转换为普通审批人节点(nodeType=4),
    /// 并标记 isCarbonCopyNode=true,使其在引擎中按普通审批人节点运行.
    /// 对应 Java NodeUtil.nodeSpecialProcess.
    /// </summary>
    public static void NodeSpecialProcess(BpmnNodeVo bpmnNodeVo)
    {
        // 上一节点指定审批人: 根据前端传入的 isPrevNodeAppointed 标识, 自动贴 af_syslabel_prev_node_appointed 标签
        if (bpmnNodeVo.IsPrevNodeAppointed == true)
        {
            bpmnNodeVo.SetOrAddLabelList(NodeLabelConstants.PrevNodeAppointed);
        }

        int? nodeType = bpmnNodeVo.NodeType == 0 ? null : (int?)bpmnNodeVo.NodeType;
        if (nodeType == null)
        {
            return;
        }

        if (nodeType == (int)NodeTypeEnum.NODE_TYPE_COPY_V2)
        {
            bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_APPROVER;
            bpmnNodeVo.IsCarbonCopyNode = true;
        }
        // 条件审批节点:设计期 nodeType=12,运行期转为 nodeType=4,标记 isConditionApproveNode
        // 保留真实审批人(不替换为虚拟审批人),仅条件满足时自动 complete
        else if (nodeType == (int)NodeTypeEnum.NODE_TYPE_CONDITION_APPROVE)
        {
            bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_APPROVER;
            bpmnNodeVo.IsConditionApproveNode = true;
        }
        // 条件抄送节点:设计期 nodeType=13,运行期转为 nodeType=4,标记 isConditionCopyNode
        // 总是 complete;仅条件满足时写 BpmProcessForward 抄送记录
        else if (nodeType == (int)NodeTypeEnum.NODE_TYPE_CONDITION_COPY)
        {
            bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_APPROVER;
            bpmnNodeVo.IsConditionCopyNode = true;
        }
    }

    /// <summary>
    /// 反显(查看流程模板)时的特殊节点标签处理.
    /// 根据节点标签将普通审批人节点还原为抄送节点v2(nodeType=8)等特殊节点类型,
    /// 以便前端按对应节点的视觉效果渲染.
    /// 对应 Java NodeUtil.nodeLabelSpecialProcess.
    /// </summary>
    public static void NodeLabelSpecialProcess(BpmnNodeVo bpmnNodeVo)
    {
        List<BpmnNodeLabelVO> labelList = bpmnNodeVo.LabelList;
        if (labelList == null || labelList.Count == 0)
        {
            return;
        }

        foreach (BpmnNodeLabelVO nodeLabelVO in labelList)
        {
            if (NodeLabelConstants.CopyNodeV2.LabelValue.Equals(nodeLabelVO.LabelValue))
            {
                bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_COPY_V2;
            }
            else if (NodeLabelConstants.ConditionApproveNode.LabelValue.Equals(nodeLabelVO.LabelValue))
            {
                bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_CONDITION_APPROVE;
            }
            else if (NodeLabelConstants.ConditionCopyNode.LabelValue.Equals(nodeLabelVO.LabelValue))
            {
                bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_CONDITION_COPY;
            }
            else if (NodeLabelConstants.PrevNodeAppointed.LabelValue.Equals(nodeLabelVO.LabelValue))
            {
                bpmnNodeVo.IsPrevNodeAppointed = true;
            }
        }
    }

    /// <summary>
    /// 为元素名称添加特殊标记后缀.
    /// 抄送节点v2添加 📢 后缀,跳过审批人添加 ⬇️ 后缀,
    /// 会签/或签/顺序会签也添加对应后缀.
    /// 对应 Java NodeUtil.elementWithSpecialMarks.
    /// </summary>
    public static void ElementWithSpecialMarks(BpmnConfCommonElementVo elementVo)
    {
        string elementName = elementVo.ElementName;

        // 当元素名为空且为用户任务,或元素名为默认名称时,根据审批人列表生成默认名称
        if ((string.IsNullOrEmpty(elementVo.ElementName)
             && ElementTypeEnum.ELEMENT_TYPE_USER_TASK.Code.Equals(elementVo.ElementType))
            || StringConstants.AF_DEFAULT_NODE_NAME.Equals(elementVo.ElementName))
        {
            IDictionary<string, string> assigneeMap = elementVo.AssigneeMap;
            if (assigneeMap != null && assigneeMap.Count > 0)
            {
                if (assigneeMap.Count <= 3)
                {
                    elementName = string.Join("|", assigneeMap.Values) + "审批";
                }
                else
                {
                    var first3AssigneeNames = assigneeMap.Values.Take(3).ToList();
                    elementName = string.Join("|", first3AssigneeNames) + "等" + assigneeMap.Count + "人审批";
                }
            }
        }

        // 会签类型后缀
        int signType = elementVo.SignType;
        if ((int)SignTypeEnum.SIGN_TYPE_SIGN == signType)
        {
            elementName += StringConstants.AF_NODE_SIGN_SUFFIX;
        }
        else if ((int)SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER == signType)
        {
            elementName += StringConstants.AF_NODE_SIGN_IN_ORDER_SUFFIX;
        }
        else if ((int)SignTypeEnum.SIGN_TYPE_OR_SIGN == signType)
        {
            elementName += StringConstants.AF_NODE_OR_SIGN_SUFFIX;
        }

        // 标签后缀
        List<BpmnNodeLabelVO> labelList = elementVo.LabelList;
        if (labelList != null && labelList.Count > 0)
        {
            bool hasCopyLabel = false;
            bool hasDeduplicationLabel = false;
            foreach (BpmnNodeLabelVO label in labelList)
            {
                if (NodeLabelConstants.CopyNodeV2.LabelValue.Equals(label.LabelValue))
                {
                    hasCopyLabel = true;
                    continue;
                }
                if (NodeLabelConstants.SkippedAssignees.LabelValue.Equals(label.LabelValue))
                {
                    hasDeduplicationLabel = true;
                }
            }
            if (hasCopyLabel)
            {
                elementName += StringConstants.AF_COPY_V2_NODE_SUFFIX;
            }
            if (hasDeduplicationLabel)
            {
                elementName += StringConstants.AF_SKIP_ASSIGNEE_NODE_SUFFIX;
            }
        }

        elementVo.ElementName = elementName;
    }
}