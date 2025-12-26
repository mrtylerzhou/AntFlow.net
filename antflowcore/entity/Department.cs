using System;

namespace antflowcore.entity;
public class Department
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ShortName { get; set; }
    public int? ParentId { get; set; }
    public string? Path { get; set; }
    public int? Level { get; set; }
    public long? LeaderId { get; set; }
    public int? Sort { get; set; }
    public bool? IsDel { get; set; }
    public bool? IsHide { get; set; }
    public string? CreateUser { get; set; }
    public string? UpdateUser { get; set; }
    public DateTime? CreateTime { get; set; } = DateTime.Now;
    public DateTime? UpdateTime { get; set; }=DateTime.Now;
}