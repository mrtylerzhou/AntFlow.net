namespace AntFlowCore.Entity;

public class BpmProcessNameRelevancy
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 流程名称 ID
    /// </summary>
    public long ProcessNameId { get; set; }

    /// <summary>
    /// 流程编号
    /// </summary>
    public string ProcessKey { get; set; }

    /// <summary>
    /// 删除标志
    /// </summary>
    public int IsDel { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }
}