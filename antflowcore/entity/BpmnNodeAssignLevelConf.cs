using System;

namespace AntFlowCore.Entity;

public class BpmnNodeAssignLevelConf
{
    public long Id { get; set; }

    public long BpmnNodeId { get; set; }

    public int? AssignLevelType { get; set; }

    public int AssignLevelGrade { get; set; }

    public string Remark { get; set; }

    public int IsDel { get; set; }
    public int? TenantId { get; set; }
    public string CreateUser { get; set; }

    public DateTime? CreateTime { get; set; }

    public string UpdateUser { get; set; }

    public DateTime? UpdateTime { get; set; }
}