namespace AntFlowCore.Base.entity
{
    /// <summary>
    /// 用户自动审批设置. 对应 Java BpmUserAutoApprove / 表 bpm_user_auto_approve.
    /// </summary>
    public class BpmUserAutoApprove
    {
        /// <summary>
        /// 主键(自增)
        /// </summary>
        public long Id { get; set; }

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
        /// 配置时活跃版本bpmnCode
        /// </summary>
        public string BpmnCode { get; set; }

        /// <summary>
        /// 节点范围JSON [{elementId,nodeName}], 空=整个流程
        /// </summary>
        public string NodeScopeJson { get; set; }

        /// <summary>
        /// 条件JSON {conditionList,groupRelation}, 仅LF
        /// </summary>
        public string ConditionJson { get; set; }

        /// <summary>
        /// 默认审批意见
        /// </summary>
        public string DefaultComment { get; set; }

        /// <summary>
        /// 启用 1是 0否
        /// </summary>
        public int Enabled { get; set; } = 1;

        public int IsDel { get; set; }
        public string TenantId { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; } = DateTime.Now;
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; } = DateTime.Now;
    }
}
