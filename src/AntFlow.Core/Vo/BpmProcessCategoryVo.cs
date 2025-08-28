using System.Text.Json.Serialization;

namespace AntFlow.Core.Vo;

public class BpmProcessCategoryVo
{
    /// <summary>
    ///     Gets or sets the ID of the process category.
    /// </summary>
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    /// <summary>
    ///     Gets or sets the name of the process type.
    /// </summary>
    [JsonPropertyName("processTypeName")]
    public string ProcessTypeName { get; set; }

    /// <summary>
    ///     Gets or sets whether the category is deleted (0 for not deleted, 1 for deleted).
    /// </summary>
    [JsonPropertyName("isDel")]
    public int? IsDel { get; set; }

    /// <summary>
    ///     Gets or sets the sort order.
    /// </summary>
    [JsonPropertyName("sort")]
    public int? Sort { get; set; }

    /// <summary>
    ///     Gets or sets whether the category is for an app (0:no, 1:yes).
    /// </summary>
    [JsonPropertyName("isApp")]
    public int? IsApp { get; set; }

    /// <summary>
    ///     Gets or sets the state of the category.
    /// </summary>
    [JsonPropertyName("state")]
    public int? State { get; set; }

    /// <summary>
    ///     Gets or sets the entrance URL.
    /// </summary>
    [JsonPropertyName("entrance")]
    public string Entrance { get; set; }

    /// <summary>
    ///     Gets or sets the type of the process category.
    /// </summary>
    [JsonPropertyName("type")]
    public int? Type { get; set; }

    /// <summary>
    ///     Gets or sets the name of the process category.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
}