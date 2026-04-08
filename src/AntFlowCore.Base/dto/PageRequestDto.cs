using System.Text.Json.Serialization;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.util;

namespace AntFlowCore.Core.dto;

public class PageRequestDto<T> where T : new()
{
    [JsonPropertyName("pageDto")]
    public PageDto PageDto { get; set; }=PageUtils.GetPageDto(new Page<object>());
    [JsonPropertyName("entity")]
    public T Entity { get; set; }
    
}