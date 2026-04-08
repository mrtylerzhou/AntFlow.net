using System.Text.Json.Serialization;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.util;
using AntFlowCore.Vo;

namespace AntFlowCore.Core.dto;

public class DetailRequestDto
{
    [JsonPropertyName("pageDto")]
    public PageDto PageDto { get; set; }= PageUtils.GetPageDto(new Page<TaskMgmtVO>());
    [JsonPropertyName("taskMgmtVO")]
    public TaskMgmtVO TaskMgmtVO { get; set; }
}