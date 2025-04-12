using System.Text.Json.Serialization;
using antflowcore.entity;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.dto;

public class DetailRequestDto
{
    [JsonPropertyName("pageDto")]
    public PageDto PageDto { get; set; }= PageUtils.GetPageDto(new Page<TaskMgmtVO>());
    [JsonPropertyName("taskMgmtVO")]
    public TaskMgmtVO TaskMgmtVO { get; set; }
}