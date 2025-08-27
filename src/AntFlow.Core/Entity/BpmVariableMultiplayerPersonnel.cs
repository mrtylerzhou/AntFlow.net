namespace AntFlow.Core.Entity;

/// <summary>
///     �����෽��Ա
/// </summary>
public class BpmVariableMultiplayerPersonnel
{
    /// <summary>
    ///     ����ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///     �����෽ID
    /// </summary>
    public long VariableMultiplayerId { get; set; }

    /// <summary>
    ///     ��ָ����
    /// </summary>
    public string Assignee { get; set; }

    /// <summary>
    ///     ��ָ��������
    /// </summary>
    public string AssigneeName { get; set; }

    /// <summary>
    ///     �Ƿ�е���0��ʾ��1��ʾ��
    /// </summary>
    public int? UndertakeStatus { get; set; }

    /// <summary>
    ///     ��ע
    /// </summary>
    public string Remark { get; set; } = "";

    /// <summary>
    ///     �Ƿ�ɾ����0��ʾ��1��ʾ��
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