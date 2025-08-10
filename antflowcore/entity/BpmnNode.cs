using System;

namespace AntFlowCore.Entity;

public class BpmnNode
{
    public long Id { get; set; }

    public long ConfId { get; set; }

    public string NodeId { get; set; }

    public int NodeType { get; set; }

    public int NodeProperty { get; set; }

    public string NodeFrom { get; set; }

    public int BatchStatus { get; set; }

    public int ApprovalStandard { get; set; }

    public string NodeName { get; set; }

    public string NodeDisplayName { get; set; }

    public string Annotation { get; set; }

    public int IsDeduplication { get; set; }

    public int IsSignUp { get; set; }

    public string Remark { get; set; }

    public int IsDel { get; set; }
    public int? TenantId { get; set; }

    public string CreateUser { get; set; }

    public DateTime? CreateTime { get; set; }

    public string UpdateUser { get; set; }

    public DateTime? UpdateTime { get; set; }

    public string NodeFroms { get; set; }

    public bool? IsDynamicCondition { get; set; }

    public bool? IsParallel { get; set; }

    // 不映射数据库字段
    public int? IsOutSideProcess { get; set; }

    public int? IsLowCodeFlow { get; set; }

    public int? ExtraFlags { get; set; }
}