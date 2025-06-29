namespace antflowcore.entity;
/// <summary>
/// Represents the third party business access table.
/// </summary>
public class OutSideBpmAccessBusiness
{
    public long Id { get; set; }
    public long BusinessPartyId { get; set; }
    public long BpmnConfId { get; set; }
    public string FormCode { get; set; }
    public string ProcessNumber { get; set; }
    public string FormDataPc { get; set; }
    public string FormDataApp { get; set; }
    public string TemplateMark { get; set; }
    public string StartUsername { get; set; }
    public string Remark { get; set; }
    public int IsDel { get; set; }
    public string CreateUser { get; set; }
    public DateTime? CreateTime { get; set; }
    public string UpdateUser { get; set; }
    public DateTime? UpdateTime { get; set; }

    public OutSideBpmAccessBusiness() { }
}