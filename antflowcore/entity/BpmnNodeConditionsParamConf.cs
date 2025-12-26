using System;
using AntFlowCore.Constants;

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
    public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;
    public int IsDel { get; set; }
    public string TenantId { get; set; }
    public string CreateUser { get; set; }
    public DateTime? CreateTime { get; set; } = DateTime.Now;
    public string UpdateUser { get; set; }
    public DateTime? UpdateTime { get; set; }=DateTime.Now;
}