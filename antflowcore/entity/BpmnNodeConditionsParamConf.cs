using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the configuration for BPMN node condition parameters.
    /// </summary>
    [Table(Name = "t_bpmn_node_conditions_param_conf")]
    public class BpmnNodeConditionsParamConf
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Condition ID.
        /// </summary>
        [Column(Name = "bpmn_node_conditions_id")]
        public long BpmnNodeConditionsId { get; set; }

        /// <summary>
        /// Condition parameter type.
        /// </summary>
        [Column(Name = "condition_param_type")]
        public int ConditionParamType { get; set; }

        /// <summary>
        /// Condition parameter name.
        /// </summary>
        [Column(Name = "condition_param_name")]
        public string ConditionParamName { get; set; }

        /// <summary>
        /// Condition parameter JSON.
        /// </summary>
        [Column(Name = "condition_param_jsom")]
        public string ConditionParamJsom { get; set; }

        /// <summary>
        /// Remarks.
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// Deletion flag (0 for normal, 1 for deleted).
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Created by user.
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Creation time.
        /// </summary>
        [Column(Name = "create_time",IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Updated by user.
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Update time.
        /// </summary>
        [Column(Name = "update_time",IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }
    }
}
