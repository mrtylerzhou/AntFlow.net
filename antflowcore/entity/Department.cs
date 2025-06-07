using FreeSql.DataAnnotations;
using System;

[Table(Name = "t_department")]
public class Department
{
    [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
    public int Id { get; set; }

    [Column(Name = "name")]
    public string? Name { get; set; }

    [Column(Name = "short_name")]
    public string? ShortName { get; set; }

    [Column(Name = "parent_id")]
    public int? ParentId { get; set; }

    [Column(Name = "path")]
    public string? Path { get; set; }

    [Column(Name = "level")]
    public int? Level { get; set; }

    [Column(Name = "leader_id")]
    public long? LeaderId { get; set; }

    [Column(Name = "sort")]
    public int? Sort { get; set; }

    [Column(Name = "is_del")]
    public bool? IsDel { get; set; }

    [Column(Name = "is_hide")]
    public bool? IsHide { get; set; }

    [Column(Name = "create_user")]
    public string? CreateUser { get; set; }

    [Column(Name = "update_user")]
    public string? UpdateUser { get; set; }

    [Column(Name = "create_time")]
    public DateTime? CreateTime { get; set; }

    [Column(Name = "update_time")]
    public DateTime? UpdateTime { get; set; }
}