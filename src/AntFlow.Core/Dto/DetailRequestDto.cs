using AntFlow.Core.Entity;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using System.Text.Json.Serialization;

namespace AntFlow.Core.Dto;

public class DetailRequestDto
{
    [JsonPropertyName("pageDto")] public PageDto PageDto { get; set; } = PageUtils.GetPageDto(new Page<TaskMgmtVO>());

    [JsonPropertyName("taskMgmtVO")] public TaskMgmtVO TaskMgmtVO { get; set; }
}