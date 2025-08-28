using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class CallbackRespVo
{
    /// <summary>
    ///     Gets or sets the response status.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; }

    /// <summary>
    ///     Gets or sets the business ID from the partner system.
    /// </summary>
    [JsonPropertyName("businessId")]
    public string BusinessId { get; set; }

    /// <summary>
    ///     Gets or sets the business party mark.
    /// </summary>
    [JsonPropertyName("businessPartyMark")]
    public string BusinessPartyMark { get; set; }

    /// <summary>
    ///     Gets or sets the extended information from the partner system.
    /// </summary>
    [JsonPropertyName("extend")]
    public string Extend { get; set; }
}