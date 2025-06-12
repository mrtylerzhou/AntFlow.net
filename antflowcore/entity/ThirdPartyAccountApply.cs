using FreeSql.DataAnnotations;

namespace antflowcore.entity;

/// <summary>
/// ThirdPartyAccountApply 实体类
/// </summary>
[Table(Name = "t_biz_account_apply")]
public class ThirdPartyAccountApply
{
    /// <summary>
    /// 主键 ID，自动递增
    /// </summary>
    [Column(IsPrimary = true, IsIdentity = true)]
    public int Id { get; set; }

    /// <summary>
    /// 账户类型
    /// </summary>
    [Column(Name = "account_type")]
    public int AccountType { get; set; }

    /// <summary>
    /// 账户所有者名称
    /// </summary>
    [Column(Name = "account_owner_name")]
    public string AccountOwnerName { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Column(Name = "remark")]
    public string Remark { get; set; }
}