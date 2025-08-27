namespace AntFlow.Core.Entity;

/// <summary>
///     Generic employee DTO, 用于表示登录员工的通用信息
/// </summary>
public class GenericEmployee
{
    /// <summary>
    ///     用户 ID
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    ///     用户名
    /// </summary>
    public string Username { get; set; } = "";

    /// <summary>
    ///     真实姓名
    /// </summary>
    public string GivenName { get; set; } = "";

    /// <summary>
    ///     工号
    /// </summary>
    public string JobNum { get; set; }

    /// <summary>
    ///     岗位名称
    /// </summary>
    public string JobName { get; set; }

    /// <summary>
    ///     岗位等级名称
    /// </summary>
    public string JobLevelName { get; set; }

    /// <summary>
    ///     照片路径
    /// </summary>
    public string PhotoPath { get; set; }

    /// <summary>
    ///     头像路径
    /// </summary>
    public string HeadImg { get; set; }

    /// <summary>
    ///     邮箱
    /// </summary>
    public string Mail { get; set; }

    /// <summary>
    ///     手机号
    /// </summary>
    public string Mobile { get; set; }

    /// <summary>
    ///     是否为系统管理员
    /// </summary>
    public bool? IsMaster { get; set; }

    /// <summary>
    ///     所属公司 ID
    /// </summary>
    public long? CompanyId { get; set; }

    /// <summary>
    ///     直接上级
    /// </summary>
    public GenericEmployee DirectLeader { get; set; }

    /// <summary>
    ///     拥有的权限码
    /// </summary>
    public HashSet<string> Permissions { get; set; } = new() { "3060101" };

    /// <summary>
    ///     汇报路径
    /// </summary>
    public string Path { get; set; }
}