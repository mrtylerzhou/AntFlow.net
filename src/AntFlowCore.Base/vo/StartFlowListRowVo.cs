namespace AntFlowCore.Base.vo;

/// <summary>
/// 发起流程页 数据聚合行. 对应 Java StartFlowListRowVo.
/// </summary>
public class StartFlowListRowVo
{
    /// <summary>
    /// 表单编码(流程 key)
    /// </summary>
    public string? FormCode { get; set; }

    /// <summary>
    /// 流程名称
    /// </summary>
    public string? BpmnName { get; set; }

    /// <summary>
    /// 流程分类 id
    /// </summary>
    public int? BpmnType { get; set; }

    /// <summary>
    /// 是否低代码流程 0/1
    /// </summary>
    public int? IsLowCodeFlow { get; set; }

    /// <summary>
    /// 是否外部流程 0/1
    /// </summary>
    public int? IsOutSideProcess { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 关联的 app 应用 id(外部流程跳转用)
    /// </summary>
    public long? ApplicationId { get; set; }

    /// <summary>
    /// 派生类型: OUTSIDE / LF / DIY
    /// </summary>
    public string? Type { get; set; }
}
