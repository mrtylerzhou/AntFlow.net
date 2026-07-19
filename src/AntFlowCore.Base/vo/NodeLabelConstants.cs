using System.Collections.Generic;
using AntFlowCore.Base.util;

namespace AntFlowCore.Base.vo
{
    /// <summary>
    /// Predefined node label constants and helper utilities.
    /// Aligned with Java NodeLabelConstants.
    /// </summary>
    public static class NodeLabelConstants
    {
        public static readonly BpmnNodeLabelVO DynamicCondition = new BpmnNodeLabelVO(StringConstants.DYNAMIC_CONDITION_NODE, "动态条件节点");
        public static readonly BpmnNodeLabelVO CopyNode = new BpmnNodeLabelVO(StringConstants.COPY_NODE, "抄送节点");
        /// <summary>
        /// 抄送节点V2版本相较于v1版本,它会真正进入到引擎,选人规则更加灵活(v1只支持指定人员),
        /// 而且能在流程图中展示出来.
        /// </summary>
        public static readonly BpmnNodeLabelVO CopyNodeV2 = new BpmnNodeLabelVO(StringConstants.COPY_NODEV2, "抄送节点V2");
        public static readonly BpmnNodeLabelVO AutomaticNode = new BpmnNodeLabelVO(StringConstants.AUTOMATIC_NODE, "自动节点");
        public static readonly BpmnNodeLabelVO SkippedAssignees = new BpmnNodeLabelVO(StringConstants.SKIPPED_ASSIGNEE, "跳过的审批人");

        /// <summary>
        /// 条件审批节点:本质是审批人节点 + 条件配置,条件满足时自动通过,否则等人工审批.
        /// 对应 Java NodeLabelConstants.conditionApproveNode.
        /// </summary>
        public static readonly BpmnNodeLabelVO ConditionApproveNode = new BpmnNodeLabelVO(StringConstants.CONDITION_APPROVE_NODE, "条件审批节点");

        /// <summary>
        /// 条件抄送节点:本质是抄送V2节点 + 条件配置,总是自动完成,仅条件满足时写抄送记录.
        /// 对应 Java NodeLabelConstants.conditionCopyNode.
        /// </summary>
        public static readonly BpmnNodeLabelVO ConditionCopyNode = new BpmnNodeLabelVO(StringConstants.CONDITION_COPY_NODE, "条件抄送节点");

        /// <summary>
        /// 上一节点指定审批人: 当前节点使用虚拟审批人 PREV_NODE_APPOINTED("-4"),
        /// 运行时由 AFTaskService.InsertTasks 替换为上一节点审批人通过[指定下一节点审批人]按钮选择的实际审批人.
        /// 对应 Java NodeLabelConstants.prevNodeAppointed.
        /// </summary>
        public static readonly BpmnNodeLabelVO PrevNodeAppointed = new BpmnNodeLabelVO(StringConstants.AF_SYSLABEL_PREV_NODE_APPOINTED, "上一节点指定审批人");

        /// <summary>
        /// 指定下一节点审批人: 贴在上一节点上,审批页据此渲染[指定下一节点审批人]按钮.
        /// 由 AbstractBpmnPersonnelAdaptor.SetNodeParams 在格式化下一节点时自动添加.
        /// 对应 Java NodeLabelConstants.appointNextNodeApprover.
        /// </summary>
        public static readonly BpmnNodeLabelVO AppointNextNodeApprover = new BpmnNodeLabelVO(StringConstants.AF_SYSLABEL_APPOINT_NEXT_NODE_APPROVER, "指定下一节点审批人");

        /// <summary>
        /// 不可操作节点,存在于引擎中,但是不可退回到的节点.
        /// 动态条件和抄送节点v1版本虽然也不可退回到,但是他们本身不会进入引擎.
        /// 条件抄送节点加入(总是自动完成);条件审批节点不加入(可能需要人工审批,支持退回).
        /// </summary>
        public static readonly List<BpmnNodeLabelVO> NoneOperationalNodes = new List<BpmnNodeLabelVO>
        {
            CopyNodeV2,
            AutomaticNode,
            ConditionCopyNode
        };

        /// <summary>
        /// Checks whether the given label list contains a label whose LabelValue
        /// equals the given labelValue.
        /// </summary>
        public static bool NodeLabelContainsAny(List<BpmnNodeLabelVO> labelList, string labelValue)
        {
            if (labelList == null || labelList.Count == 0 || string.IsNullOrEmpty(labelValue))
            {
                return false;
            }
            foreach (var label in labelList)
            {
                if (labelValue.Equals(label.LabelValue))
                {
                    return true;
                }
            }
            return false;
        }
    }
}