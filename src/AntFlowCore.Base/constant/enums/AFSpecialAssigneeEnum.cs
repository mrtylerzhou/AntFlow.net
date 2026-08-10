using AntFlowCore.Base.vo;

namespace AntFlowCore.Base.constant.enums;

public class AFSpecialAssigneeEnum
{
    public int Code { get; }
    public string Id { get; }
    public string Desc { get; }

    private AFSpecialAssigneeEnum(int code, string id, string desc)
    {
        Code = code;
        Id = id;
        Desc = desc;
    }

    public static readonly AFSpecialAssigneeEnum TO_BE_REMOVED = new AFSpecialAssigneeEnum(0, "0", "最终会被去除的人员");
    public static readonly AFSpecialAssigneeEnum COPY_NODE = new AFSpecialAssigneeEnum(1, "-1", "流程通知");
    public static readonly AFSpecialAssigneeEnum CC_NODE = new AFSpecialAssigneeEnum(-1, "-1", "抄送人");
    public static readonly AFSpecialAssigneeEnum SKIP = new AFSpecialAssigneeEnum(-2, "-2", "自动节点自动跳过");
    /// <summary>
    /// 自动节点虚拟审批人: 设计时作为自动节点的 assignee,运行时由 BpmnTaskListener 自动完成任务
    /// </summary>
    public static readonly AFSpecialAssigneeEnum AUTO_NODE_SKIP = new AFSpecialAssigneeEnum(-3, "-3", "自动节点自动跳过");
    /// <summary>
    /// 上一节点指定的审批人: 虚拟用户, 运行时由 AFTaskService.InsertTasks 替换为实际审批人
    /// </summary>
    public static readonly AFSpecialAssigneeEnum PREV_NODE_APPOINTED = new AFSpecialAssigneeEnum(-4, "-4", "上一节点指定的审批人");
    /// <summary>
    /// 到达前设置(动态审批人): 虚拟用户, 设计时作为节点 assignee 透传;
    /// 运行到该节点时由 NextNodeDynamicAssigneeProcessor 调用
    /// IFormOperationAdaptor.ProvideCurrentNodeAssignees 动态查询真实审批人并委托.
    /// </summary>
    public static readonly AFSpecialAssigneeEnum ARRIVAL_DYNAMIC_ASSIGNEE = new AFSpecialAssigneeEnum(-5, "-5", "到达前动态查询审批人");
    public static IEnumerable<AFSpecialAssigneeEnum> Values
    {
        get
        {
            yield return TO_BE_REMOVED;
            yield return COPY_NODE;
            yield return CC_NODE;
            yield return SKIP;
            yield return AUTO_NODE_SKIP;
            yield return PREV_NODE_APPOINTED;
            yield return ARRIVAL_DYNAMIC_ASSIGNEE;
        }
    }

    /// <summary>
    /// 返回所有特殊指派人列表
    /// </summary>
    public static List<BaseIdTranStruVo> GetAllSpecialAssignees()
    {
        var result = new List<BaseIdTranStruVo>();
        foreach (var value in Values)
        {
            result.Add(new BaseIdTranStruVo(value.Id, value.Desc));
        }
        return result;
    }

    /// <summary>
    /// 根据 id 查找特殊指派人
    /// </summary>
    public static BaseIdTranStruVo GetSpecialAssignee(string id)
    {
        foreach (var value in Values)
        {
            if (value.Id.Equals(id))
            {
                return new BaseIdTranStruVo(value.Id, value.Desc);
            }
        }
        return null;
    }
}
