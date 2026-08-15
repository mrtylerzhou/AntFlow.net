namespace AntFlowCore.Base.entity;

/// <summary>
/// 流程权限记录. 对应 Java BpmProcessPermissions(统一对象模型: objectType + objectId).
/// 对象类型: 1=人员 2=部门 3=角色
/// </summary>
public class BpmProcessPermissions
{
    /// <summary>
    /// Auto-increment ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 授权对象类型 1=人员 2=部门 3=角色
    /// </summary>
    public int? ObjectType { get; set; }

    /// <summary>
    /// 授权对象 id(人员id/部门id/角色id)
    /// </summary>
    public string? ObjectId { get; set; }

    /// <summary>
    /// Permission type:
    /// 1: View
    /// 2: Create
    /// 3: Monitor
    /// </summary>
    public int? PermissionsType { get; set; }

    /// <summary>
    /// Create user id.
    /// </summary>
    public string? CreateUser { get; set; }

    /// <summary>
    /// Create time.
    /// </summary>
    public DateTime? CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Process key (form code).
    /// </summary>
    public string? ProcessKey { get; set; }

    /// <summary>
    /// Deletion Status (0: Not Deleted, 1: Deleted)
    /// </summary>
    public int IsDel { get; set; }

    public string? TenantId { get; set; }
}