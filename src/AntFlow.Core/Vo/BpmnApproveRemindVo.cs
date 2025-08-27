using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class BpmnApproveRemindVo
{
    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("confId")] public long ConfId { get; set; }

    [JsonPropertyName("nodeId")] public long NodeId { get; set; }

    [JsonPropertyName("isInuse")] public bool? IsInuse { get; set; }

    [JsonPropertyName("templateId")] public long? TemplateId { get; set; }

    [JsonPropertyName("templateName")] public string TemplateName { get; set; }

    [JsonPropertyName("days")] public string Days { get; set; }

    [JsonPropertyName("dayList")] public List<int> DayList { get; set; }

    [JsonPropertyName("isDel")] public int IsDel { get; set; }

    [JsonPropertyName("createTime")] public DateTime? CreateTime { get; set; }

    [JsonPropertyName("createUser")] public string CreateUser { get; set; }

    [JsonPropertyName("updateTime")] public DateTime? UpdateTime { get; set; }

    [JsonPropertyName("updateUser")] public string UpdateUser { get; set; }
}