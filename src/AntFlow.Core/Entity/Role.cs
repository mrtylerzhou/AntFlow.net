using FreeSql.DataAnnotations;
using System.Text.Json.Serialization;

namespace AntFlow.Core.Entity;

[Table(Name = "t_user")]
public class Role
{
    /// <summary>
    ///     ???ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///     ???????
    /// </summary>
    [JsonPropertyName("roleName")]
    public string RoleName { get; set; }
}