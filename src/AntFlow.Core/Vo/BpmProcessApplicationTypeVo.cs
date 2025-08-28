using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class BpmProcessApplicationTypeVo
{
    /// <summary>
    ///     Gets or sets the application ID.
    /// </summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>
    ///     Gets or sets the application ID.
    /// </summary>
    [JsonPropertyName("applicationId")]
    public long? ApplicationId { get; set; }

    /// <summary>
    ///     Gets or sets the category ID.
    /// </summary>
    [JsonPropertyName("categoryId")]
    public long? CategoryId { get; set; }

    /// <summary>
    ///     Gets or sets whether the item is deleted (0 for not deleted, 1 for deleted).
    /// </summary>
    [JsonPropertyName("isDel")]
    public int? IsDel { get; set; }

    /// <summary>
    ///     Gets or sets the list of process types.
    /// </summary>
    [JsonPropertyName("processTypes")]
    public List<long>? ProcessTypes { get; set; }

    /// <summary>
    ///     Gets or sets the sort order.
    /// </summary>
    [JsonPropertyName("sort")]
    public int? Sort { get; set; }

    /// <summary>
    ///     Gets or sets the type of the application.
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    ///     Gets or sets whether the application is frequently used (0 for no, 1 for yes).
    /// </summary>
    [JsonPropertyName("state")]
    public int? State { get; set; }

    /// <summary>
    ///     Gets or sets the frequently used ID.
    /// </summary>
    [JsonPropertyName("commonUseId")]
    public long? CommonUseId { get; set; }

    /// <summary>
    ///     Gets or sets the history ID.
    /// </summary>
    [JsonPropertyName("historyId")]
    public long? HistoryId { get; set; }

    /// <summary>
    ///     Gets or sets the visibility state (0 for hidden, 1 for visible).
    /// </summary>
    [JsonPropertyName("visbleState")]
    public int? VisbleState { get; set; }

    /// <summary>
    ///     Gets or sets the creation time.
    /// </summary>
    [JsonPropertyName("createTime")]
    public DateTime? CreateTime { get; set; }

    /// <summary>
    ///     Gets or sets the frequently used state.
    /// </summary>
    [JsonPropertyName("commonUseState")]
    public int? CommonUseState { get; set; }
}