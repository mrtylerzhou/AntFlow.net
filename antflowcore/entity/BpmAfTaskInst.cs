using FreeSql.DataAnnotations;
using Microsoft.VisualBasic.CompilerServices;

namespace antflowcore.entity;

 /// <summary>
    /// 流程任务实例实体类
    /// </summary>
    [Table(Name = "bpm_af_taskinst")]
    public class BpmAfTaskInst
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column(Name = "id", IsPrimary = true)]
        public string Id { get; set; }

        /// <summary>
        /// 流程定义 ID
        /// </summary>
        [Column(Name = "proc_def_id")]
        public string ProcDefId { get; set; }

        /// <summary>
        /// 任务定义键
        /// </summary>
        [Column(Name = "task_def_key")]
        public string TaskDefKey { get; set; }

        /// <summary>
        /// 流程实例 ID
        /// </summary>
        [Column(Name = "proc_inst_id")]
        public string ProcInstId { get; set; }

        /// <summary>
        /// 执行实例 ID
        /// </summary>
        [Column(Name = "execution_id")]
        public string ExecutionId { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        [Column(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// 父任务 ID
        /// </summary>
        [Column(Name = "parent_task_id")]
        public string ParentTaskId { get; set; }

        /// <summary>
        /// 任务所有者
        /// </summary>
        [Column(Name = "owner")]
        public string Owner { get; set; }

        /// <summary>
        /// 任务受托人
        /// </summary>
        [Column(Name = "assignee")]
        public string Assignee { get; set; }

        /// <summary>
        /// 受托人姓名
        /// </summary>
        [Column(Name = "assignee_name")]
        public string AssigneeName { get; set; }
        /// <summary>
        /// 原始处理人
        /// </summary>
        [Column(Name = "original_assignee")]
        public string OriginalAssignee { get; set; }
        /// <summary>
        /// 原始处理人姓名
        /// </summary>
        [Column(Name = "original_assignee_name")]
        public string OriginalAssigneeName { get; set; }
        
        /// <summary>
        /// 处理人变更原因
        /// </summary>
        [Column(Name = "transfer_reason")]
        public string TransferReason { get; set; }
        /// <summary>
        /// 审批状态
        /// </summary>
        [Column(Name = "verify_status")]
        public int VerifyStatus { get; set; }
        /// <summary>
        /// 审核意见
        /// </summary>
        [Column(Name = "verify_desc")]
        public string VerifyDesc { get; set; }
        /// <summary>
        /// 任务开始时间
        /// </summary>
        [Column(Name = "start_time")]
        public DateTime StartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 任务认领时间
        /// </summary>
        [Column(Name = "claim_time")]
        public DateTime? ClaimTime { get; set; }

        /// <summary>
        /// 任务结束时间
        /// </summary>
        [Column(Name = "end_time")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 持续时间（毫秒）
        /// </summary>
        [Column(Name = "duration")]
        public long? Duration { get; set; }

        /// <summary>
        /// 删除原因
        /// </summary>
        [Column(Name = "delete_reason")]
        public string DeleteReason { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [Column(Name = "priority")]
        public int? Priority { get; set; }

        /// <summary>
        /// 到期时间
        /// </summary>
        [Column(Name = "due_date")]
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// 表单标识
        /// </summary>
        [Column(Name = "form_key")]
        public string FormKey { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        [Column(Name = "category")]
        public string Category { get; set; }

        /// <summary>
        /// 租户 ID
        /// </summary>
        [Column(Name = "tenant_id")]
        public string TenantId { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        [Column(Name = "description")]
        public string Description { get; set; }
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }
    }