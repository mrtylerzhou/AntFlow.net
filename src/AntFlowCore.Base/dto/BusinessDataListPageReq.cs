namespace AntFlowCore.Base.dto;

/// <summary>
/// 业务数据列表分页请求. 对应 Java BusinessDataListPageReq.
/// </summary>
public class BusinessDataListPageReq
{
    public PageDto? PageDto { get; set; }

    /// <summary>
    /// 低代码流程 formCode
    /// </summary>
    public string? FormCode { get; set; }

    /// <summary>
    /// 流程编号关键字(模糊)
    /// </summary>
    public string? ProcessNumber { get; set; }
}