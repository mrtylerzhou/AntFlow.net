using System;
using FreeSql;
using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a BPMN node.
    /// </summary>
    [Table(Name = "t_bpmn_node")]
    public class BpmnNode
    {
        /// <summary>
        /// Gets or sets the auto-incrementing ID.
        /// </summary>
        [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the configuration ID.
        /// </summary>
        [Column(Name = "conf_id")]
        public long ConfId { get; set; }

        /// <summary>
        /// Gets or sets the node ID.
        /// </summary>
        [Column(Name = "node_id")]
        public string NodeId { get; set; }

        /// <summary>
        /// Gets or sets the node type (NodeTypeEnum).
        /// </summary>
        [Column(Name = "node_type")]
        public int NodeType { get; set; }

        /// <summary>
        /// Gets or sets the node property (NodePropertyEnum).
        /// </summary>
        [Column(Name = "node_property")]
        public int NodeProperty { get; set; }

        /// <summary>
        /// Gets or sets the previous node.
        /// </summary>
        [Column(Name = "node_from")]
        public string NodeFrom { get; set; }

        /// <summary>
        /// Gets or sets the batch approval status (0 for no, 1 for yes).
        /// </summary>
        [Column(Name = "batch_status")]
        public int BatchStatus { get; set; }

        /// <summary>
        /// Gets or sets the approval standard (1 for start user, 2 for approval).
        /// </summary>
        [Column(Name = "approval_standard")]
        public int ApprovalStandard { get; set; }

        /// <summary>
        /// Gets or sets the node name.
        /// </summary>
        [Column(Name = "node_name")]
        public string NodeName { get; set; }

        /// <summary>
        /// Gets or sets the node display name.
        /// </summary>
        [Column(Name = "node_display_name")]
        public string NodeDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the annotation.
        /// </summary>
        [Column(Name = "annotation")]
        public string Annotation { get; set; }

        /// <summary>
        /// Gets or sets whether deduplication is enabled (0 for no, 1 for yes).
        /// </summary>
        [Column(Name = "is_deduplication")]
        public int IsDeduplication { get; set; }

        /// <summary>
        /// Gets or sets whether the node is a sign-up node (0 for no, 1 for yes).
        /// </summary>
        [Column(Name = "is_sign_up")]
        public int IsSignUp { get; set; }

        /// <summary>
        /// Gets or sets the remark.
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// Gets or sets the deletion status (0 for normal, 1 for delete).
        /// </summary>
        [Column(Name = "is_del")]
        public int IsDel { get; set; }

        /// <summary>
        /// Gets or sets the user who created the node.
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// Gets or sets the time the node was created.
        /// </summary>
        [Column(Name = "create_time",IsIgnore = true)]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Gets or sets the user who last updated the node.
        /// </summary>
        [Column(Name = "update_user")]
        public string UpdateUser { get; set; }

        /// <summary>
        /// Gets or sets the time the node was last updated.
        /// </summary>
        [Column(Name = "update_time",IsIgnore = true)]
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// Gets or sets the list of previous nodes.
        /// </summary>
        [Column(Name = "node_froms")]
        public string NodeFroms { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether this node is part of an outside process.
        /// </summary>
        [Column(IsIgnore = true)]
        public int? IsOutSideProcess { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether this node is part of a low-code flow.
        /// </summary>
        [Column(IsIgnore = true)]
        public int? IsLowCodeFlow { get; set; }
        [Column(IsIgnore = true)]
        public int? ExtraFlags { get; set; }
    }
}
