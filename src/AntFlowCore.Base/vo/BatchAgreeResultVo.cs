using System.Text.Json.Serialization;

namespace AntFlowCore.Base.vo;

/// <summary>
/// 批量同意结果VO
/// </summary>
public class BatchAgreeResultVo
{
    [JsonPropertyName("successCount")]
    public int SuccessCount { get; set; }

    [JsonPropertyName("failures")]
    public List<FailureItem> Failures { get; set; } = new();

    public class FailureItem
    {
        [JsonPropertyName("taskId")]
        public string TaskId { get; set; }

        [JsonPropertyName("processNumber")]
        public string ProcessNumber { get; set; }

        [JsonPropertyName("processName")]
        public string ProcessName { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }
    }
}
