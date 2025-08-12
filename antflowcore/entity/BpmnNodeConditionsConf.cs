using System;

namespace AntFlowCore.Entity;

public class BpmnNodeConditionsConf
{
    public long Id { get; set; }
    public long BpmnNodeId { get; set; }
    public int? IsDefault { get; set; }
    public int? Sort { get; set; }
    public string ExtJson { get; set; }
    public int? GroupRelation { get; set; }
    public string Remark { get; set; }
    public int IsDel { get; set; }
    public string TenantId { get; set; }
    public string CreateUser { get; set; }
    public DateTime? CreateTime { get; set; }
    public string UpdateUser { get; set; }
    public DateTime? UpdateTime { get; set; }
}