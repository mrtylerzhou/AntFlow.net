using FreeSql.DataAnnotations;

namespace antflowcore.entity;

/// <summary>
/// 字典主表实体类
/// </summary>
public class DictMain
{
    public long Id { get; set; }
    public string DictName { get; set; }
    public string DictType { get; set; }
    public int? IsDel { get; set; }
    public string TenantId { get; set; }
    public DateTime? CreateTime { get; set; }
    public string CreateUser { get; set; }
    public DateTime? UpdateTime { get; set; }
    public string UpdateUser { get; set; }
    public string Remark { get; set; }
}