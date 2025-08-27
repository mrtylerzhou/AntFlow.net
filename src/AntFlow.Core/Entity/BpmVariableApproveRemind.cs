namespace AntFlow.Core.Entity;

/// <summary>
///     ���̱�����������
/// </summary>
public class BpmVariableApproveRemind
{
    /// <summary>
    ///     �������� ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///     ���̱��� ID
    /// </summary>
    public long VariableId { get; set; }

    /// <summary>
    ///     ����Ԫ�� ID
    /// </summary>
    public string ElementId { get; set; }

    /// <summary>
    ///     ��������
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    ///     ��ע
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    ///     ɾ����־
    /// </summary>
    public int IsDel { get; set; }

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