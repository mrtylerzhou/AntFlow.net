namespace AntFlow.Core.Entity;

/// <summary>
///     ��������ʵ��ʵ����
/// </summary>
public class BpmAfTaskInst
{
    /// <summary>
    ///     ����
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///     ���̶��� ID
    /// </summary>
    public string ProcDefId { get; set; }

    /// <summary>
    ///     �������
    /// </summary>
    public string TaskDefKey { get; set; }

    /// <summary>
    ///     ����ʵ�� ID
    /// </summary>
    public string ProcInstId { get; set; }

    /// <summary>
    ///     ִ��ʵ�� ID
    /// </summary>
    public string ExecutionId { get; set; }

    /// <summary>
    ///     ��������
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     ������ ID
    /// </summary>
    public string ParentTaskId { get; set; }

    /// <summary>
    ///     ����������
    /// </summary>
    public string Owner { get; set; }

    /// <summary>
    ///     ����������
    /// </summary>
    public string Assignee { get; set; }

    /// <summary>
    ///     ����������
    /// </summary>
    public string AssigneeName { get; set; }

    /// <summary>
    ///     ԭʼ������
    /// </summary>
    public string OriginalAssignee { get; set; }

    /// <summary>
    ///     ԭʼ����������
    /// </summary>
    public string OriginalAssigneeName { get; set; }

    /// <summary>
    ///     �����˱��ԭ��
    /// </summary>
    public string TransferReason { get; set; }

    /// <summary>
    ///     ����״̬
    /// </summary>
    public int VerifyStatus { get; set; }

    /// <summary>
    ///     ������
    /// </summary>
    public string VerifyDesc { get; set; }

    /// <summary>
    ///     ����ʼʱ��
    /// </summary>
    public DateTime StartTime { get; set; } = DateTime.Now;

    /// <summary>
    ///     ��������ʱ��
    /// </summary>
    public DateTime? ClaimTime { get; set; }

    /// <summary>
    ///     �������ʱ��
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    ///     ����ʱ�䣨���룩
    /// </summary>
    public long? Duration { get; set; }

    /// <summary>
    ///     ɾ��ԭ��
    /// </summary>
    public string DeleteReason { get; set; }

    /// <summary>
    ///     ���ȼ�
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    ///     ����ʱ��
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    ///     ����ʶ
    /// </summary>
    public string FormKey { get; set; }

    /// <summary>
    ///     ���
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    ///     �⻧ ID
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    ///     ������Ϣ
    /// </summary>
    public string Description { get; set; }

    public string UpdateUser { get; set; }
}