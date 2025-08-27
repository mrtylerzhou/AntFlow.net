namespace AntFlow.Core.Entity;

/// <summary>
///     变量单一指派实体
/// </summary>
public class BpmVariableSingle
{
    // 无参构造函数
    public BpmVariableSingle() { }

    // 带参构造函数
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
    ///     自增ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///     变量ID
    /// </summary>
    public long VariableId { get; set; }

    /// <summary>
    ///     元素ID
    /// </summary>
    public string ElementId { get; set; }

    /// <summary>
    ///     节点ID
    /// </summary>
    public string NodeId { get; set; }

    /// <summary>
    ///     元素名称
    /// </summary>
    public string ElementName { get; set; }

    /// <summary>
    ///     单一指派参数名称
    /// </summary>
    public string AssigneeParamName { get; set; }

    /// <summary>
    ///     被指派人
    /// </summary>
    public string Assignee { get; set; }

    /// <summary>
    ///     被指派人姓名
    /// </summary>
    public string AssigneeName { get; set; }

    /// <summary>
    ///     备注
    /// </summary>
    public string Remark { get; set; } = "";

    /// <summary>
    ///     是否删除，0表示否，1表示是
    /// </summary>
    public int IsDel { get; set; }

    public string TenantId { get; set; }

    /// <summary>
    ///     创建用户
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    ///     创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    ///     更新用户
    /// </summary>
    public string UpdateUser { get; set; }

    /// <summary>
    ///     更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}