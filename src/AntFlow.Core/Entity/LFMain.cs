namespace AntFlow.Core.Entity;

public class LFMain
{
    /// <summary>
    ///     ����ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///     �����ȡ�������õĶ����ֶ�
    /// </summary>
    public long? ConfId { get; set; }

    /// <summary>
    ///     �����ȡ������Ķ����ֶ�
    /// </summary>
    public string FormCode { get; set; }

    /// <summary>
    ///     �߼�ɾ����ǣ�0��δɾ����1����ɾ����
    /// </summary>
    public int IsDel { get; set; }

    public string TenantId { get; set; }

    /// <summary>
    ///     ������
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    ///     ����ʱ��
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    ///     ������
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    ///     ����ʱ��
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}