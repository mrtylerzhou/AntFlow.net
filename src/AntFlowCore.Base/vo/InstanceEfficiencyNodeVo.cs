namespace AntFlowCore.Base.vo;

/// <summary>
/// 流程实例效能-节点列表项 VO
/// </summary>
public class InstanceEfficiencyNodeVo
{
    /// <summary>
    /// 任务定义 Key(BPMN element id)
    /// </summary>
    public string TaskDefKey { get; set; }

    /// <summary>
    /// 节点名称
    /// </summary>
    public string NodeName { get; set; }

    /// <summary>
    /// 节点类型
    /// </summary>
    public int? NodeType { get; set; }

    public string NodeTypeName { get; set; }

    /// <summary>
    /// 节点总耗时(毫秒,退回多轮累加)
    /// </summary>
    public long Duration { get; set; }

    /// <summary>
    /// 节点总耗时(格式化文本)
    /// </summary>
    public string DurationText { get; set; }

    /// <summary>
    /// 是否发生过退回(多轮)
    /// </summary>
    public bool HasRollback { get; set; }

    /// <summary>
    /// 是否进行中(当前运行节点)
    /// </summary>
    public bool InProgress { get; set; }

    /// <summary>
    /// TOP 排名(1/2/3,null 表示未上榜)
    /// 进行中节点不参与排行
    /// </summary>
    public int? TopRank { get; set; }

    /// <summary>
    /// 执行序号(按 min(start_time) 升序)
    /// </summary>
    public int OrderNo { get; set; }
}
