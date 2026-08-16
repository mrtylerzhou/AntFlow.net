using AntFlowCore.Base.util;

namespace AntFlowCore.Base.entity;

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

    /// <summary>
    /// 抗去重: 为 true 时该节点不参与审批人去重(自身不被去重, 其审批人也不作为其他节点的去重基准).
    /// 与 Java 版一致, 落库到 t_bpmn_node.deduplicationExclude 列, 读取时经 MapperUtil 拷到 BpmnNodeVo.
    /// </summary>
    public bool DeduplicationExclude { get; set; }

    public int IsSignUp { get; set; }

    public string Remark { get; set; } = StringConstants.BIG_WHITE_BLANK;

    public int IsDel { get; set; }
    public string TenantId { get; set; }

    public string CreateUser { get; set; }

    public DateTime? CreateTime { get; set; } = DateTime.Now;

    public string UpdateUser { get; set; }

    public DateTime? UpdateTime { get; set; }=DateTime.Now;

    public string NodeFroms { get; set; }

    public bool? IsDynamicCondition { get; set; }

    public bool? IsParallel { get; set; }

    // 不映射数据库字段
    public int? IsOutSideProcess { get; set; }

    public int? IsLowCodeFlow { get; set; }

    public int? ConfExtraFlags { get; set; }
    public int?  NoHeaderAction { get; set; }
    
    public int? ExtraFlags { get; set; }

    /// <summary>
    /// Consolidated node-level JSON configuration.
    /// </summary>
    public string NodeConfigJson { get; set; }
    
}
