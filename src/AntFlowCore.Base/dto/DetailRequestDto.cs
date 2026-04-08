using System.Text.Json.Serialization;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Base.dto;

public class DetailRequestDto
{
    [JsonPropertyName("pageDto")]
    public PageDto PageDto { get; set; }= PageUtils.GetPageDto(new Page<TaskMgmtVO>());
    [JsonPropertyName("taskMgmtVO")]
    public TaskMgmtVO TaskMgmtVO { get; set; }
}