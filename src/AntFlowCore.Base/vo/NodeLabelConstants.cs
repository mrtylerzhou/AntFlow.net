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
        /// 条件推进节点:条件审批(nodeType=12)子类型,自动勾选推进按钮(42,别名同意),强制 forwardType=2.
        /// 满足条件时自动推进到固定目标,不满足时留给真实审批人(点"同意"=推进到固定目标).对应 Java NodeLabelConstants.conditionAdvanceNode.
        /// </summary>
        public static readonly BpmnNodeLabelVO ConditionAdvanceNode = new BpmnNodeLabelVO(StringConstants.CONDITION_ADVANCE_NODE, "条件推进节点");

        /// <summary>
        /// 条件完成节点:条件推进(nodeType=12)子类型,目标设计时自动算最后一个审批人节点,不可编辑.
        /// 运行时复用条件推进处理器.对应 Java NodeLabelConstants.conditionFinishNode.
        /// </summary>
        public static readonly BpmnNodeLabelVO ConditionFinishNode = new BpmnNodeLabelVO(StringConstants.CONDITION_FINISH_NODE, "条件完成节点");

        /// <summary>
        /// 条件抄送节点:本质是抄送V2节点 + 条件配置,总是自动完成,仅条件满足时写抄送记录.
        /// 对应 Java NodeLabelConstants.conditionCopyNode.
        /// </summary>
        public static readonly BpmnNodeLabelVO ConditionCopyNode = new BpmnNodeLabelVO(StringConstants.CONDITION_COPY_NODE, "条件抄送节点");

        /// <summary>
        /// 选择条件节点:审批人节点+动态条件网关组合,运行时审批人选择后续条件分支.
        /// 对应 Java NodeLabelConstants.pickCondition.
        /// </summary>
        public static readonly BpmnNodeLabelVO PickCondition = new BpmnNodeLabelVO(StringConstants.AF_SYSLABEL_PICK_CONDITION, "选择条件节点");

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
        /// 不同意退回: 运行时EndProcessService据此标签判断是否转发BackToModifyService.
        /// 对应 Java NodeLabelConstants.disagreeBack.
        /// </summary>
        public static readonly BpmnNodeLabelVO DisagreeBack = new BpmnNodeLabelVO(StringConstants.AF_SYSLABEL_DISAGREE_BACK, "不同意退回");

        /// <summary>退回按钮行为:退回发起人. 对应 Java NodeLabelConstants.backInitiator.</summary>
        public static readonly BpmnNodeLabelVO BackInitiator = new BpmnNodeLabelVO(StringConstants.AF_SYSLABEL_BACK_INITIATOR, "退回发起人");
        /// <summary>退回按钮行为:退回上一节点. 对应 Java NodeLabelConstants.backPrev.</summary>
        public static readonly BpmnNodeLabelVO BackPrev = new BpmnNodeLabelVO(StringConstants.AF_SYSLABEL_BACK_PREV, "退回上一节点");
        /// <summary>退回按钮行为:退回指定节点. 对应 Java NodeLabelConstants.backSpecified.</summary>
        public static readonly BpmnNodeLabelVO BackSpecified = new BpmnNodeLabelVO(StringConstants.AF_SYSLABEL_BACK_SPECIFIED, "退回指定节点");

        /// <summary>
        /// 协助节点:本质是审批人节点,语义为"办理"而非"审批",不代表同意/不同意,但流程仍需向下流转.
        /// 对应 Java NodeLabelConstants.assistNode.
        /// </summary>
        public static readonly BpmnNodeLabelVO AssistNode = new BpmnNodeLabelVO(StringConstants.ASSIST_NODE, "协助节点");

        /// <summary>自动推进节点:满足条件时推进到指定目标节点,不满足时和自动节点一样 complete.对应 Java NodeLabelConstants.autoAdvanceNode.</summary>
        public static readonly BpmnNodeLabelVO AutoAdvanceNode = new BpmnNodeLabelVO(StringConstants.AUTO_ADVANCE_NODE, "自动推进节点");

        /// <summary>
        /// 自动完成节点:自动推进(nodeType=18)子类型,目标自动为最后一个审批人节点,不可编辑.
        /// 仅前端反显区分+颜色区分用,运行时复用 auto_advance_node 处理器.对应 Java NodeLabelConstants.autoCompleteNode.
        /// </summary>
        public static readonly BpmnNodeLabelVO AutoCompleteNode = new BpmnNodeLabelVO(StringConstants.AUTO_COMPLETE_NODE, "自动完成节点");

        /// <summary>自动退回节点:满足条件时退回到指定目标节点(FOUR_DISAGREE),不满足时和自动节点一样 complete.对应 Java NodeLabelConstants.autoReturnNode.</summary>
        public static readonly BpmnNodeLabelVO AutoReturnNode = new BpmnNodeLabelVO(StringConstants.AUTO_RETURN_NODE, "自动退回节点");

        /// <summary>完成审批节点:审批人节点+推进按钮,目标自动填充为流程最后一个审批人节点.对应 Java NodeLabelConstants.finishApproveNode.</summary>
        public static readonly BpmnNodeLabelVO FinishApproveNode = new BpmnNodeLabelVO(StringConstants.FINISH_APPROVE_NODE, "完成审批节点");

        /// <summary>
        /// 不可操作节点,存在于引擎中,但是不可退回到的节点.
        /// 动态条件和抄送节点v1版本虽然也不可退回到,但是他们本身不会进入引擎.
        /// 条件抄送节点加入(总是自动完成);条件审批节点不加入(可能需要人工审批,支持退回).
        /// </summary>
        public static readonly List<BpmnNodeLabelVO> NoneOperationalNodes = new List<BpmnNodeLabelVO>
        {
            CopyNodeV2,
            AutomaticNode,
            ConditionCopyNode,
            AutoAdvanceNode,
            AutoCompleteNode
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