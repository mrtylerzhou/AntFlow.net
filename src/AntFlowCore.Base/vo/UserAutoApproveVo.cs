using AntFlowCore.Base.vo;

namespace AntFlowCore.Base.vo
{
    /// <summary>
    /// 用户自动审批设置 前后端交换VO. 对应 Java UserAutoApproveVo.
    /// </summary>
    public class UserAutoApproveVo
    {
        public long? Id { get; set; }

        /// <summary>
        /// 配置指向版本的bpmnConf id(编辑时拉节点下拉用)
        /// </summary>
        public long? ConfId { get; set; }

        /// <summary>
        /// 归属人id
        /// </summary>
        public string OwnerUserId { get; set; }

        /// <summary>
        /// 归属人姓名
        /// </summary>
        public string OwnerUserName { get; set; }

        /// <summary>
        /// 流程formCode
        /// </summary>
        public string FormCode { get; set; }

        /// <summary>
        /// 配置指向的版本bpmnCode
        /// </summary>
        public string BpmnCode { get; set; }

        /// <summary>
        /// 流程名称(展示用)
        /// </summary>
        public string BpmnName { get; set; }

        /// <summary>
        /// 流程类型 1 DIY 2 LF低代码 3 第三方
        /// </summary>
        public int? FlowType { get; set; }

        /// <summary>
        /// 节点范围, 空=整个流程
        /// </summary>
        public List<NodeScopeItem> NodeScope { get; set; }

        /// <summary>
        /// 审批条件(后端存储格式), 仅LF
        /// </summary>
        public List<List<BpmnNodeConditionsConfVueVo>> ConditionList { get; set; }

        /// <summary>
        /// 条件组关系 false=且 true=或
        /// </summary>
        public bool? GroupRelation { get; set; }

        /// <summary>
        /// 默认审批意见
        /// </summary>
        public string DefaultComment { get; set; }

        /// <summary>
        /// 启用 1是 0否
        /// </summary>
        public int? Enabled { get; set; }

        /// <summary>
        /// 活跃状态(实时计算列): 配置bpmnCode == 当前活跃版本
        /// </summary>
        public bool? Active { get; set; }

        public DateTime? CreateTime { get; set; }

        public class NodeScopeItem
        {
            /// <summary>
            /// 节点elementId
            /// </summary>
            public string ElementId { get; set; }

            /// <summary>
            /// 节点名称快照
            /// </summary>
            public string NodeName { get; set; }
        }
    }
}
