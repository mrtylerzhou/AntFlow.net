using FreeSql.DataAnnotations;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents a BPM process node submission.
    /// </summary>
    [Table(Name = "bpm_process_node_submit")]
    public class BpmProcessNodeSubmit
    {
        /// <summary>
        /// Auto-increment ID.
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        /// <summary>
        /// Process instance ID.
        /// </summary>
        [Column(Name = "processInstance_Id")]
        public string ProcessInstanceId { get; set; }

        /// <summary>
        /// Back type:
        /// 1: Back to previous node and commit to next node
        /// 2: Back to initiator and commit to next node
        /// 3: Back to initiator and commit to back node
        /// 4: Back to history node and commit to next node
        /// 5: Back to history node and commit to back node
        /// </summary>
        [Column(Name = "back_type")]
        public int BackType { get; set; }

        /// <summary>
        /// Node key.
        /// </summary>
        [Column(Name = "node_key")]
        public string NodeKey { get; set; }

        /// <summary>
        /// Creation time.
        /// </summary>
        [Column(Name = "create_time")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// Creation user.
        /// </summary>
        [Column(Name = "create_user")]
        public string CreateUser { get; set; }

        /// <summary>
        /// State.
        /// </summary>
        [Column(Name = "state")]
        public int State { get; set; }
    }
}