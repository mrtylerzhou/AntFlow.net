using System.Text.Json.Serialization;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;

namespace AntFlowCore.Base.dto;

public class PageRequestDto<T> where T : new()
{
    [JsonPropertyName("pageDto")]
    public PageDto PageDto { get; set; }=PageUtils.GetPageDto(new Page<object>());
    [JsonPropertyName("entity")]
    public T Entity { get; set; }
    
}