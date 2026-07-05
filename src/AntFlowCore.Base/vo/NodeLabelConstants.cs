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
        /// 不可操作节点,存在于引擎中,但是不可退回到的节点.
        /// 动态条件和抄送节点v1版本虽然也不可退回到,但是他们本身不会进入引擎.
        /// </summary>
        public static readonly List<BpmnNodeLabelVO> NoneOperationalNodes = new List<BpmnNodeLabelVO>
        {
            CopyNodeV2,
            AutomaticNode
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
