namespace AntFlow.Core.Entity;

/// <summary>
///     Represents the conditions template for the external BPM system.
/// </summary>
public class OutSideBpmConditionsTemplate
{
    public long Id { get; set; }
    public long BusinessPartyId { get; set; }
    public string TemplateMark { get; set; }
    public string TemplateName { get; set; }
    public int ApplicationId { get; set; }
    public string Remark { get; set; }
    public int IsDel { get; set; }
    public string CreateUser { get; set; }
    public DateTime? CreateTime { get; set; }
    public string UpdateUser { get; set; }
    public DateTime? UpdateTime { get; set; }
    public string CreateUserId { get; set; }
}