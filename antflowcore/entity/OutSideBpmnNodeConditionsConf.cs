namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the node conditions configuration for the external BPM system.
    /// </summary>
    public class OutSideBpmnNodeConditionsConf
    {
        public long Id { get; set; }
        public long BpmnNodeId { get; set; }
        public string OutSideId { get; set; }
        public string Remark { get; set; }
        public int IsDel { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}