namespace antflowcore.entity;

public class BpmProcessOperation
{
    public long Id { get; set; }
    public string? ProcessKey { get; set; }
    public string? ProcessNode { get; set; }

    /// <summary>
    /// 1: batch submit, 2: entrust
    /// </summary>
    public int? Type { get; set; }
}