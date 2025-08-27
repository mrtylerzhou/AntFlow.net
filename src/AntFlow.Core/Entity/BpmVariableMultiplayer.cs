namespace AntFlow.Core.Entity;

/// <summary>
///     Represents a BPM variable multiplayer.
/// </summary>
public class BpmVariableMultiplayer
{
    /// <summary>
    ///     ����ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///     ����ID
    /// </summary>
    public long VariableId { get; set; }

    /// <summary>
    ///     Ԫ��ID
    /// </summary>
    public string ElementId { get; set; }

    /// <summary>
    ///     �ڵ�ID
    /// </summary>
    public string NodeId { get; set; }

    /// <summary>
    ///     Ԫ������
    /// </summary>
    public string ElementName { get; set; }

    /// <summary>
    ///     ��������
    /// </summary>
    public string CollectionName { get; set; }

    /// <summary>
    ///     ǩ������ 1��ʾȫǩ��2��ʾ��ǩ
    /// </summary>
    public int SignType { get; set; }

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

    /// <summary>
    ///     ����״̬������ӳ��
    /// </summary>
    public int? UnderTakeStatus { get; set; }
}