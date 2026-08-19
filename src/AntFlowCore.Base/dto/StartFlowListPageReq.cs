namespace AntFlowCore.Base.dto;

/// <summary>
/// 发起流程页 分页请求参数. 对应 Java StartFlowListPageReq.
/// </summary>
public class StartFlowListPageReq
{
    /// <summary>
    /// 第几页(页 = 最多 3 栏, 栏内按分类块)
    /// </summary>
    public int? Page { get; set; }

    /// <summary>
    /// 流程名称过滤
    /// </summary>
    public string? BpmnName { get; set; }

    /// <summary>
    /// formCode 过滤
    /// </summary>
    public string? FormCode { get; set; }

    /// <summary>
    /// 流程类型(分类)id, -1 表示未分类
    /// </summary>
    public long? CategoryId { get; set; }
}
