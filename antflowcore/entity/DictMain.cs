using FreeSql.DataAnnotations;

namespace antflowcore.entity;

/// <summary>
/// 字典主表实体类
/// </summary>
[Table(Name = "t_dict_main")]
public class DictMain
{
    /// <summary>
    /// 主键ID（自增）
    /// </summary>
    [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
    public long Id { get; set; }
 
    /// <summary>
    /// 字典名称（必填，最大长度100）
    /// </summary>
    [Column(Name = "dict_name")]
    public string DictName { get; set; }
 
    /// <summary>
    /// 字典类型（必填，最大长度50）
    /// </summary>
    [Column(Name = "dict_type")]
  
    public string DictType { get; set; }
 
    /// <summary>
    /// 是否删除（0-未删除 1-已删除）
    /// </summary>
    [Column(Name = "is_del")]
    public int? IsDel { get; set; }
 
    /// <summary>
    /// 创建时间
    /// </summary>
    [Column(Name = "create_time")]
    public DateTime? CreateTime { get; set; }
 
    /// <summary>
    /// 创建人（最大长度50）
    /// </summary>
    [Column(Name = "create_user")]
    public string CreateUser { get; set; }
 
    /// <summary>
    /// 更新时间
    /// </summary>
    [Column(Name = "update_time")]
    public DateTime? UpdateTime { get; set; }
 
    /// <summary>
    /// 更新人（最大长度50）
    /// </summary>
    [Column(Name = "update_user")]
    public string UpdateUser { get; set; }
 
    /// <summary>
    /// 备注信息（最大长度500）
    /// </summary>
    [Column(Name = "remark")]
    public string Remark { get; set; }
}