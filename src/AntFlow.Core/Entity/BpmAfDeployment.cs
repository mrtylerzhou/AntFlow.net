namespace AntFlow.Core.Entity;

/// <summary>
///     BpmAfDeployment Entity
/// </summary>
public class BpmAfDeployment
{
    /// <summary>
    ///     ����
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///     �޶���
    /// </summary>
    public int? Rev { get; set; }

    /// <summary>
    ///     ��������
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     ��������
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    ///     ��ע
    /// </summary>
    public string Remark { get; set; } = "";

    /// <summary>
    ///     �Ƿ�ɾ��
    /// </summary>
    public bool IsDel { get; set; }

    public string TenantId { get; set; }

    /// <summary>
    ///     �����û�
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    ///     ����ʱ��
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    ///     �����û�
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    ///     ����ʱ��
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}