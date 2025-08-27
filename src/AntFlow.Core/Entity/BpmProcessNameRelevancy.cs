namespace AntFlow.Core.Entity;

public class BpmProcessNameRelevancy
{
    /// <summary>
    ///     ���� ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///     �������� ID
    /// </summary>
    public long ProcessNameId { get; set; }

    /// <summary>
    ///     ���̱��
    /// </summary>
    public string ProcessKey { get; set; }

    /// <summary>
    ///     ɾ����־
    /// </summary>
    public int IsDel { get; set; }

    public string TenantId { get; set; }

    /// <summary>
    ///     ����ʱ��
    /// </summary>
    public DateTime? CreateTime { get; set; }
}