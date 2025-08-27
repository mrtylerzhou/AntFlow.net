namespace AntFlow.Core.Entity;

/// <summary>
///     Represents the third party process service, business party admin person.
/// </summary>
public class OutSideBpmAdminPersonnel
{
    public long Id { get; set; }
    public long BusinessPartyId { get; set; }
    public int Type { get; set; }
    public string EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string Remark { get; set; }
    public int IsDel { get; set; }
    public string CreateUser { get; set; }
    public DateTime? CreateTime { get; set; }
    public string UpdateUser { get; set; }
    public DateTime? UpdateTime { get; set; }
}