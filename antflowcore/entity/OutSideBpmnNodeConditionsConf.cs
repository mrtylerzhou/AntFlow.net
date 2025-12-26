using AntFlowCore.Constants;

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
        public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;
        public int IsDel { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; } = DateTime.Now;
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }=DateTime.Now;
    }
}