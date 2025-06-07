using FreeSql.DataAnnotations;

namespace antflowcore.entity;

[Table(Name = "t_user_role")]
public class UserRole
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public long? Id { get; set; }

    [Column(Name = "user_id")]
    public long? UserId { get; set; }
    [Column(Name = "role_id")]
    public long? RoleId { get; set; }
}