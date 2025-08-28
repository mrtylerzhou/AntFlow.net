namespace AntFlow.Core.Entity;

/// <summary>
///     ������һָ��ʵ��
/// </summary>
public class BpmVariableSingle
{
    // �޲ι��캯��
    public BpmVariableSingle() { }

    // ���ι��캯��
    public BpmVariableSingle(long id, long variableId, string elementId, string nodeId, string elementName,
        string assigneeParamName, string assignee, string assigneeName, string remark, int isDel,
        string createUser, DateTime? createTime, string updateUser, DateTime? updateTime)
    {
        Id = id;
        VariableId = variableId;
        ElementId = elementId;
        NodeId = nodeId;
        ElementName = elementName;
        AssigneeParamName = assigneeParamName;
        Assignee = assignee;
        AssigneeName = assigneeName;
        Remark = remark;
        IsDel = isDel;
        CreateUser = createUser;
        CreateTime = createTime;
        UpdateUser = updateUser;
        UpdateTime = updateTime;
    }

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
    ///     ��һָ�ɲ�������
    /// </summary>
    public string AssigneeParamName { get; set; }

    /// <summary>
    ///     ��ָ����
    /// </summary>
    public string Assignee { get; set; }

    /// <summary>
    ///     ��ָ��������
    /// </summary>
    public string AssigneeName { get; set; }

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