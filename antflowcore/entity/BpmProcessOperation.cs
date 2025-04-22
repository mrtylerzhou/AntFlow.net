namespace antflowcore.entity;

using FreeSql.DataAnnotations;

[Table(Name = "bpm_process_operation")]
public class BpmProcessOperation
{
    [Column(Name = "id", IsPrimary = true, IsIdentity = true)]
    public long Id { get; set; }

    [Column(Name = "process_key")]
    public string? ProcessKey { get; set; }

    [Column(Name = "process_node")]
    public string? ProcessNode { get; set; }

    /// <summary>
    /// 1: batch submit, 2: entrust
    /// </summary>
    [Column(Name = "type")]
    public int? Type { get; set; }
}
