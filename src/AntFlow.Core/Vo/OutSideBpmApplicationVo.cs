using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class OutSideBpmApplicationVo
{
    /// <summary>
    ///     第三方业务id
    /// </summary>
    [JsonPropertyName("thirdId")]
    public long ThirdId { get; set; }

    /// <summary>
    ///     第三方业务名称
    /// </summary>
    [JsonPropertyName("thirdName")]
    public string ThirdName { get; set; }

    /// <summary>
    ///     第三方业务code
    /// </summary>
    [JsonPropertyName("thirdCode")]
    public string ThirdCode { get; set; }

    /// <summary>
    ///     第三方业务备注
    /// </summary>
    [JsonPropertyName("thirdRemark")]
    public string ThirdRemark { get; set; }

    /// <summary>
    ///     流程名称
    /// </summary>
    [JsonPropertyName("processName")]
    public string ProcessName { get; set; }

    /// <summary>
    ///     流程key
    /// </summary>
    [JsonPropertyName("processKey")]
    public string ProcessKey { get; set; }
}