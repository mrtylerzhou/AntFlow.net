using AntFlowCore.Constants;

namespace antflowcore.entity
{
    /// <summary>
    /// Third party process service, condition template conf
    /// </summary>
    public class OutSideBpmApproveTemplate
    {
        public long Id { get; set; }
        public long? BusinessPartyId { get; set; }
        public int? ApplicationId { get; set; }
        public int? ApproveTypeId { get; set; }
        public string ApproveTypeName { get; set; }
        public string ApiClientId { get; set; }
        public string ApiClientSecret { get; set; }
        public string ApiToken { get; set; }
        public string ApiUrl { get; set; }
        public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;
        public int? IsDel { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; } = DateTime.Now;
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }=DateTime.Now;
        public string CreateUserId { get; set; }
    }
}