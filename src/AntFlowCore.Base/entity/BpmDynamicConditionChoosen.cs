namespace AntFlowCore.Base.entity
{
    /// <summary>
    /// Records which dynamic condition branch was chosen for a process instance.
    /// Used to detect condition changes during resubmit (dynamic condition migration).
    /// </summary>
    public class BpmDynamicConditionChoosen
    {
        public long Id { get; set; }

        /// <summary>
        /// Process number (business number).
        /// </summary>
        public string ProcessNumber { get; set; }

        /// <summary>
        /// The condition node id that was chosen (matched).
        /// </summary>
        public string NodeId { get; set; }

        /// <summary>
        /// The gateway node id (nodeFrom of the condition node).
        /// </summary>
        public string NodeFrom { get; set; }
    }
}
