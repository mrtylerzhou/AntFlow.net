using FreeSql.DataAnnotations;

namespace antflowcore.entity;

[Table(Name = "t_dict_data")]
public class DictData
{
    [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
    public long Id { get; set; }
 
    [Column(Name = "dict_sort")]
    public int? Sort { get; set; }
 
    [Column(Name = "dict_label")]
    public string Label { get; set; }
 
    [Column(Name = "dict_value")]
    public string Value { get; set; }
 
    [Column(Name = "dict_type")]
    public string DictType { get; set; }
 
    [Column(Name = "css_class")]
    public string CssClass { get; set; }
 
    [Column(Name = "list_class")]
    public string ListClass { get; set; }
 
    [Column(Name = "is_default")]
    public string IsDefault { get; set; }
 
    [Column(Name = "is_del")]
    public int? IsDel { get; set; }
 
    [Column(Name = "create_time")]
    public DateTime? CreateTime { get; set; }
 
    [Column(Name = "create_user")]
    public string CreateUser { get; set; }
 
    [Column(Name = "update_time")]
    public DateTime? UpdateTime { get; set; }
 
    [Column(Name = "update_user")]
    public string UpdateUser { get; set; }
 
    [Column(Name = "remark")]
    public string Remark { get; set; }
}