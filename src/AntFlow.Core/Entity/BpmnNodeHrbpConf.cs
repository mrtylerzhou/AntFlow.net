namespace AntFlow.Core.Entity;

public class BpmnNodeHrbpConf
{
    public long Id { get; set; }
    public long BpmnNodeId { get; set; }
    public int? HrbpConfType { get; set; }
    public string Remark { get; set; }
    public int IsDel { get; set; }
    public string TenantId { get; set; }
    public string CreateUser { get; set; }
    public DateTime? CreateTime { get; set; }
    public string UpdateUser { get; set; }
    public DateTime? UpdateTime { get; set; }
}