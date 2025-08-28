namespace AntFlow.Core.Entity;

/// <summary>
///     Represents an external BPM business party.
/// </summary>
public class OutSideBpmBusinessParty
{
    public long Id { get; set; }
    public string BusinessPartyMark { get; set; }
    public string Name { get; set; }
    public int Type { get; set; }
    public string Remark { get; set; }
    public int IsDel { get; set; }
    public string CreateUser { get; set; }
    public DateTime? CreateTime { get; set; }
    public string UpdateUser { get; set; }
    public DateTime? UpdateTime { get; set; }
}