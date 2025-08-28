using System.Text.Json.Serialization;
using System;

namespace AntFlow.Core.Vo;

public class BpmnTimeoutReminderTaskVo
{
    [JsonPropertyName("procInstId")]
    public string ProcInstId { get; set; }

    [JsonPropertyName("taskId")]
    public string TaskId { get; set; }

    [JsonPropertyName("elementId")]
    public string ElementId { get; set; }

    [JsonPropertyName("assignee")]
    public string Assignee { get; set; }

    [JsonPropertyName("createTime")]
    public DateTime? CreateTime { get; set; }

    [JsonPropertyName("standbyDay")]
    public int StandbyDay { get; set; }
}