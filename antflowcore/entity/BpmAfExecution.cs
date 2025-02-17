using FreeSql.DataAnnotations;

namespace antflowcore.entity;

/// <summary>
    /// 流程执行实例实体类
    /// </summary>
    [Table(Name = "bpm_af_execution")]
    [Index("AF_IDX_EXEC_PROCINSTID", nameof(ProcInstId))] // 流程实例 ID 索引
    [Index("AF_IDX_EXEC_BUSKEY", nameof(BusinessKey))] // 业务键索引
    public class BpmAfExecution
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column(Name = "id", IsPrimary = true)]
        public string Id { get; set; }

        /// <summary>
        /// 修订版本
        /// </summary>
        [Column(Name = "rev_")]
        public int? Rev { get; set; }

        /// <summary>
        /// 流程实例 ID
        /// </summary>
        [Column(Name = "proc_inst_id")]
        public string ProcInstId { get; set; }

        /// <summary>
        /// 业务键
        /// </summary>
        [Column(Name = "business_key")]
        public string BusinessKey { get; set; }

        /// <summary>
        /// 父执行 ID
        /// </summary>
        [Column(Name = "parent_id")]
        public string ParentId { get; set; }

        /// <summary>
        /// 流程定义 ID
        /// </summary>
        [Column(Name = "proc_def_id")]
        public string ProcDefId { get; set; }

        /// <summary>
        /// 超级执行 ID
        /// </summary>
        [Column(Name = "super_exec")]
        public string SuperExec { get; set; }

        /// <summary>
        /// 根流程实例 ID
        /// </summary>
        [Column(Name = "root_proc_inst_id")]
        public string RootProcInstId { get; set; }

        /// <summary>
        /// 当前活动 ID
        /// </summary>
        [Column(Name = "act_id")]
        public string ActId { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        [Column(Name = "is_active")]
        public bool? IsActive { get; set; }

        /// <summary>
        /// 是否并发
        /// </summary>
        [Column(Name = "is_concurrent")]
        public bool? IsConcurrent { get; set; }

        /// <summary>
        /// 租户 ID
        /// </summary>
        [Column(Name = "tenant_id")]
        public string TenantId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Column(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Column(Name = "start_time")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 启动用户 ID
        /// </summary>
        [Column(Name = "start_user_id")]
        public string StartUserId { get; set; }

        /// <summary>
        /// 是否启用计数
        /// </summary>
        [Column(Name = "is_count_enabled")]
        public bool? IsCountEnabled { get; set; }

        /// <summary>
        /// 事件订阅计数
        /// </summary>
        [Column(Name = "evt_subscr_count")]
        public int? EvtSubscrCount { get; set; }

        /// <summary>
        /// 任务计数
        /// </summary>
        [Column(Name = "task_count")]
        public int? TaskCount { get; set; }

        /// <summary>
        /// 变量计数
        /// </summary>
        [Column(Name = "var_count")]
        public int? VarCount { get; set; }
    }