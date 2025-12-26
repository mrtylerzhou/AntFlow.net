using System;
using AntFlowCore.Constants;

namespace AntFlowCore.Entity;

public class BpmnNodeConditionsConf
{
    public long Id { get; set; }
    public long BpmnNodeId { get; set; }
    public int? IsDefault { get; set; }
    public int? Sort { get; set; }
    public string ExtJson { get; set; }
    public int? GroupRelation { get; set; }
    public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;
    public int IsDel { get; set; }
    public string TenantId { get; set; }
    public string CreateUser { get; set; }
    public DateTime? CreateTime { get; set; } = DateTime.Now;
    public string UpdateUser { get; set; }
    public DateTime? UpdateTime { get; set; }=DateTime.Now;
}