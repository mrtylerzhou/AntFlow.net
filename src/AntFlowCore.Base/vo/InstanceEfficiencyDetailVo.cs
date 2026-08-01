namespace AntFlowCore.Base.vo;

/// <summary>
/// 流程实例效能-节点详情 VO
/// </summary>
public class InstanceEfficiencyDetailVo
{
    public string TaskDefKey { get; set; }

    public string NodeName { get; set; }

    /// <summary>
    /// 节点类型
    /// </summary>
    public int? NodeType { get; set; }

    public string NodeTypeName { get; set; }

    /// <summary>
    /// 人员来源类型
    /// </summary>
    public int? NodeProperty { get; set; }

    public string NodePropertyName { get; set; }

    /// <summary>
    /// 签署类型
    /// </summary>
    public int? SignType { get; set; }

    public string SignTypeName { get; set; }

    /// <summary>
    /// 是否发生过退回
    /// </summary>
    public bool HasRollback { get; set; }

    /// <summary>
    /// 最后一轮人员明细
    /// </summary>
    public List<InstanceEfficiencyAssigneeVo> Assignees { get; set; }
}
