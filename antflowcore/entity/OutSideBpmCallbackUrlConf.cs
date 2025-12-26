using AntFlowCore.Constants;

namespace AntFlowCore.Entity
{
    /// <summary>
    /// Represents the configuration for external BPM business party callback URLs.
    /// </summary>
    public class OutSideBpmCallbackUrlConf
    {
        public long Id { get; set; }
        public long BusinessPartyId { get; set; }
        public long? ApplicationId { get; set; }
        public long BpmnConfId { get; set; }
        public string FormCode { get; set; }
        public string BpmConfCallbackUrl { get; set; }
        public string BpmFlowCallbackUrl { get; set; }
        public string ApiClientId { get; set; }
        public string ApiClientSecret { get; set; }
        public int Status { get; set; }
        public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;
        public int IsDel { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; } = DateTime.Now;
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }=DateTime.Now;
    }
}