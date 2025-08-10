using System;

namespace AntFlowCore.Entity;

public class BpmnNodeBusinessTableConf
{
    public long Id { get; set; }
    public long BpmnNodeId { get; set; }
    public int? ConfigurationTableType { get; set; }
    public int? TableFieldType { get; set; }
    public int? SignType { get; set; }
    public string Remark { get; set; }
    public int IsDel { get; set; }
    public string TenantId { get; set; }
    public string CreateUser { get; set; }
    public DateTime? CreateTime { get; set; }
    public string UpdateUser { get; set; }
    public DateTime? UpdateTime { get; set; }
}