namespace AntFlowCore.Base.entity
{
    /// <summary>
    /// Process draft entity. Stores a serialized form data snapshot so that a user can
    /// save a draft of a process form before submitting it, and load it later.
    /// Only the latest draft per (processKey, createUser) is kept.
    /// </summary>
    public class BpmBusinessDraft
    {
        public long Id { get; set; }

        /// <summary>
        /// The bpmn code (template code) associated with the draft.
        /// </summary>
        public string BpmnCode { get; set; }

        public DateTime? CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Process number (not populated at draft time).
        /// </summary>
        public string ProcessCode { get; set; }

        public string CreateUserName { get; set; }

        public string CreateUser { get; set; }

        /// <summary>
        /// The form code (a.k.a. processKey) the draft belongs to.
        /// </summary>
        public string ProcessKey { get; set; }

        /// <summary>
        /// The serialized form data (JSON) of the draft.
        /// </summary>
        public string DraftJson { get; set; }

        public int IsDel { get; set; }

        public string TenantId { get; set; }
    }
}
