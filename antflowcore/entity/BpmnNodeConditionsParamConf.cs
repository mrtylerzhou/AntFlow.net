using System;

namespace AntFlowCore.Entity;

public class BpmnNodeConditionsParamConf
{
    public long Id { get; set; }
    public long BpmnNodeConditionsId { get; set; }
    public int ConditionParamType { get; set; }
    public string ConditionParamName { get; set; }
    public string ConditionParamJsom { get; set; }
    public int? TheOperator { get; set; }
    public int? CondRelation { get; set; }
    public int? CondGroup { get; set; }
    public string Remark { get; set; }
    public int IsDel { get; set; }
    public int? TenantId { get; set; }
    public string CreateUser { get; set; }
    public DateTime? CreateTime { get; set; }
    public string UpdateUser { get; set; }
    public DateTime? UpdateTime { get; set; }
}