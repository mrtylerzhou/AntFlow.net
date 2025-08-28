namespace AntFlow.Core.Entity;

/// <summary>
///     ����ִ��ʵ��ʵ����
/// </summary>
public class BpmAfExecution
{
    /// <summary>
    ///     ����
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///     �޶��汾
    /// </summary>
    public int? Rev { get; set; }

    /// <summary>
    ///     ����ʵ�� ID
    /// </summary>
    public string ProcInstId { get; set; }

    /// <summary>
    ///     ҵ���
    /// </summary>
    public string BusinessKey { get; set; }

    /// <summary>
    ///     ��ִ�� ID
    /// </summary>
    public string ParentId { get; set; }

    /// <summary>
    ///     ���̶��� ID
    /// </summary>
    public string ProcDefId { get; set; }

    /// <summary>
    ///     ����ִ�� ID
    /// </summary>
    public string SuperExec { get; set; }

    /// <summary>
    ///     ������ʵ�� ID
    /// </summary>
    public string RootProcInstId { get; set; }

    /// <summary>
    ///     ��ǰ� ID
    /// </summary>
    public string ActId { get; set; }

    /// <summary>
    ///     �Ƿ񼤻�
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    ///     �Ƿ񲢷�
    /// </summary>
    public bool? IsConcurrent { get; set; }

    /// <summary>
    ///     �⻧ ID
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    ///     ����
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     ��ʼʱ��
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    ///     �����û� ID
    /// </summary>
    public string StartUserId { get; set; }

    /// <summary>
    ///     �Ƿ����ü���
    /// </summary>
    public bool? IsCountEnabled { get; set; }

    /// <summary>
    ///     �¼����ļ���
    /// </summary>
    public int? EvtSubscrCount { get; set; }

    /// <summary>
    ///     �������
    /// </summary>
    public int? TaskCount { get; set; }

    /// <summary>
    ///     ��������
    /// </summary>
    public int? VarCount { get; set; }

    /// <summary>
    ///     ǩ������
    /// </summary>
    public int? SignType { get; set; }
}