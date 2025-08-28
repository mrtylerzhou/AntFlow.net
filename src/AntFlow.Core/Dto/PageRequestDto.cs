using AntFlow.Core.Entity;
using AntFlow.Core.Util;
using System.Text.Json.Serialization;

namespace AntFlow.Core.Dto;

public class PageRequestDto<T> where T : new()
{
    [JsonPropertyName("pageDto")] public PageDto PageDto { get; set; } = PageUtils.GetPageDto(new Page<object>());

    [JsonPropertyName("entity")] public T Entity { get; set; }
}