using FreeSql.DataAnnotations;

namespace antflowcore.entity;

[Table(Name = "tab_student")]
public class Student
{
    [Column(IsPrimary = true)]
    public int Id { get; set; }

    [Column(Name = "name")]
    public string Name { get; set; }

    [Column(IsIgnore = true)]
    public int Age { get; set; }
}