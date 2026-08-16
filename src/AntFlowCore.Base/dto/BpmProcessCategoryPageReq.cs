using System.Text.Json.Serialization;
using AntFlowCore.Base.dto;

namespace AntFlowCore.Base.dto;

/// <summary>
/// 流程分类管理列表查询请求. 对应 Java BpmProcessCategoryPageReq.
/// </summary>
public class BpmProcessCategoryPageReq
{
    [JsonPropertyName("pageDto")]
    public PageDto? PageDto { get; set; }

    /// <summary>
    /// 分类名称(模糊)
    /// </summary>
    [JsonPropertyName("processTypeName")]
    public string? ProcessTypeName { get; set; }
}
