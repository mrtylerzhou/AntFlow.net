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

        // 退回按钮行为: 根据前端传入的 DrawBackType 自动贴对应标签
        int? drawBackType = bpmnNodeVo.DrawBackType;
        if (drawBackType != null && drawBackType != 0)
        {
            if (drawBackType == 2 || drawBackType == 3)
            {
                bpmnNodeVo.SetOrAddLabelList(NodeLabelConstants.BackInitiator);
            }
            else if (drawBackType == 1)
            {
                bpmnNodeVo.SetOrAddLabelList(NodeLabelConstants.BackPrev);
            }
            else if (drawBackType == 4 || drawBackType == 5)
            {
                bpmnNodeVo.SetOrAddLabelList(NodeLabelConstants.BackSpecified);
            }
        }

        // 完成审批节点:根据前端传入的 IsFinishApproveNode 标识自动贴标签
        // 完成审批本质是审批人节点+推进按钮,但目标自动填充为最后一个审批人节点
        if (bpmnNodeVo.IsFinishApproveNode == true)
        {
            bpmnNodeVo.SetOrAddLabelList(NodeLabelConstants.FinishApproveNode);
        }

        // 自动完成节点:根据前端传入的 IsAutoCompleteNode 标识自动贴标签
        // 自动完成本质是自动推进(nodeType=18)子类型,目标自动为最后一个审批人,运行时复用 auto_advance_node 处理器
        // 此标签仅用于前端反显区分+颜色区分
        if (bpmnNodeVo.IsAutoCompleteNode == true)
        {
            bpmnNodeVo.SetOrAddLabelList(NodeLabelConstants.AutoCompleteNode);
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
        // 自动节点:设计期 nodeType=9,运行期转为 nodeType=4,标记 isAutomaticNode
        // 设置虚拟审批人 AUTO_NODE_SKIP(-3)
        else if (nodeType == (int)NodeTypeEnum.NODE_TYPE_AUTO_NODE)
        {
            bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_APPROVER;
            bpmnNodeVo.IsAutomaticNode = true;
            bpmnNodeVo.NodeProperty = (int)NodePropertyEnum.NODE_PROPERTY_PERSONNEL;
            // 设置虚拟审批人 AUTO_NODE_SKIP(-3)
            AddOrEditProperty(bpmnNodeVo, prop =>
            {
                prop.SignType ??= 1;
                if (prop.EmplIds == null || prop.EmplIds.Count == 0)
                    prop.EmplIds = new List<string> { AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id };
                if (prop.EmplList == null || prop.EmplList.Count == 0)
                    prop.EmplList = new List<BaseIdTranStruVo>
                    {
                        new BaseIdTranStruVo(AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id, AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Desc)
                    };
            });
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
        // 协助节点:设计期 nodeType=17,运行期转为 nodeType=4,标记 isAssistNode
        // 语义为"办理"而非"审批",保留真实办理人,不塞虚拟审批人
        else if (nodeType == (int)NodeTypeEnum.NODE_TYPE_ASSIST)
        {
            bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_APPROVER;
            bpmnNodeVo.IsAssistNode = true;
        }
        // 自动推进节点:设计期 nodeType=18,运行期转为 nodeType=4,标记 isAutoAdvanceNode
        // 与自动节点(9)同构:强制指定人员 + 塞虚拟审批人 -3
        // 差异:满足条件时推进到指定目标节点,不满足时和自动节点一样 complete
        else if (nodeType == (int)NodeTypeEnum.NODE_TYPE_AUTO_ADVANCE)
        {
            bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_APPROVER;
            bpmnNodeVo.IsAutoAdvanceNode = true;
            bpmnNodeVo.NodeProperty = (int)NodePropertyEnum.NODE_PROPERTY_PERSONNEL;
            AddOrEditProperty(bpmnNodeVo, prop =>
            {
                prop.SignType ??= 1;
                if (prop.EmplIds == null || prop.EmplIds.Count == 0)
                    prop.EmplIds = new List<string> { AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id };
                if (prop.EmplList == null || prop.EmplList.Count == 0)
                    prop.EmplList = new List<BaseIdTranStruVo>
                    {
                        new BaseIdTranStruVo(AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id, AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Desc)
                    };
            });
        }
        // 自动退回节点:设计期 nodeType=19,运行期转为 nodeType=4,标记 isAutoReturnNode
        // 与自动推进(18)同构:强制指定人员 + 塞虚拟审批人 -3
        // 差异:满足条件时退回到指定目标节点(FOUR_DISAGREE),不满足时和自动节点一样 complete
        else if (nodeType == (int)NodeTypeEnum.NODE_TYPE_AUTO_RETURN)
        {
            bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_APPROVER;
            bpmnNodeVo.IsAutoReturnNode = true;
            bpmnNodeVo.NodeProperty = (int)NodePropertyEnum.NODE_PROPERTY_PERSONNEL;
            AddOrEditProperty(bpmnNodeVo, prop =>
            {
                prop.SignType ??= 1;
                if (prop.EmplIds == null || prop.EmplIds.Count == 0)
                    prop.EmplIds = new List<string> { AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id };
                if (prop.EmplList == null || prop.EmplList.Count == 0)
                    prop.EmplList = new List<BaseIdTranStruVo>
                    {
                        new BaseIdTranStruVo(AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id, AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Desc)
                    };
            });
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
            else if (NodeLabelConstants.ConditionAdvanceNode.LabelValue.Equals(nodeLabelVO.LabelValue))
            {
                // 条件推进节点:条件审批(nodeType=12)子类型,自动勾选推进按钮(42,别名同意),强制 forwardType=2
                // 还原 nodeType=12 并标记 IsConditionAdvanceNode, 供前端反显推进设置tab/图标/颜色
                bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_CONDITION_APPROVE;
                bpmnNodeVo.IsConditionAdvanceNode = true;
            }
            else if (NodeLabelConstants.ConditionFinishNode.LabelValue.Equals(nodeLabelVO.LabelValue))
            {
                // 条件完成节点:条件推进(nodeType=12)子类型,目标自动算最后一个审批人,不可编辑
                // 还原 nodeType=12 并标记 IsConditionFinishNode, 供前端反显
                bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_CONDITION_APPROVE;
                bpmnNodeVo.IsConditionFinishNode = true;
            }
            else if (NodeLabelConstants.ConditionCopyNode.LabelValue.Equals(nodeLabelVO.LabelValue))
            {
                bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_CONDITION_COPY;
            }
            else if (NodeLabelConstants.AutomaticNode.LabelValue.Equals(nodeLabelVO.LabelValue))
            {
                bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_AUTO_NODE;
            }
            else if (NodeLabelConstants.PrevNodeAppointed.LabelValue.Equals(nodeLabelVO.LabelValue))
            {
                bpmnNodeVo.IsPrevNodeAppointed = true;
            }
            else if (NodeLabelConstants.PickCondition.LabelValue.Equals(nodeLabelVO.LabelValue))
            {
                bpmnNodeVo.IsPickCondition = true;
            }
            else if (NodeLabelConstants.AssistNode.LabelValue.Equals(nodeLabelVO.LabelValue))
            {
                bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_ASSIST;
            }
            else if (NodeLabelConstants.AutoAdvanceNode.LabelValue.Equals(nodeLabelVO.LabelValue))
            {
                bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_AUTO_ADVANCE;
            }
            else if (NodeLabelConstants.AutoReturnNode.LabelValue.Equals(nodeLabelVO.LabelValue))
            {
                bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_AUTO_RETURN;
            }
            else if (NodeLabelConstants.AutoCompleteNode.LabelValue.Equals(nodeLabelVO.LabelValue))
            {
                // 自动完成节点:本质是自动推进(18)子类型,带 auto_complete_node 标签,目标自动为最后一个审批人
                // 还原 nodeType=18, 标记 IsAutoCompleteNode 供前端反显区分+颜色区分
                bpmnNodeVo.NodeType = (int)NodeTypeEnum.NODE_TYPE_AUTO_ADVANCE;
                bpmnNodeVo.IsAutoCompleteNode = true;
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