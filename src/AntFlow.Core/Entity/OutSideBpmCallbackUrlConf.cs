namespace AntFlow.Core.Entity;

/// <summary>
///     Represents the configuration for external BPM business party callback URLs.
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
    public string Remark { get; set; }
    public int IsDel { get; set; }
    public string CreateUser { get; set; }
    public DateTime? CreateTime { get; set; }
    public string UpdateUser { get; set; }
    public DateTime? UpdateTime { get; set; }
}