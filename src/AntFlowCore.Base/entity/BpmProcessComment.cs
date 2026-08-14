namespace AntFlowCore.Base.entity;

/// <summary>
/// 流程沟通表.
/// 按流程实例(ProcessNumber)一条会话, 支持二级回复(ParentId/RootId 扁平存).
/// 对齐 Java 版 jimuoffice 的 BpmProcessComment / t_bpm_process_comment.
/// </summary>
public class BpmProcessComment
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 流程实例编号(会话锚点)
    /// </summary>
    public string ProcessNumber { get; set; }

    /// <summary>
    /// 回复哪条消息(根消息为 null)
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// 所属根消息 id(根=自身; 回复归到根, 二级分组用, 避免递归回溯)
    /// </summary>
    public long? RootId { get; set; }

    /// <summary>
    /// 消息正文
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 图片/附件 url JSON 数组(仅预留字段, v1 不做上传)
    /// </summary>
    public string Attachment { get; set; }

    /// <summary>
    /// @提及 JSON [{userId,userName}]
    /// </summary>
    public string Mentions { get; set; }

    /// <summary>
    /// 回复目标人 userId(回复消息时填)
    /// </summary>
    public string ReplyToUser { get; set; }

    /// <summary>
    /// 回复目标人姓名快照
    /// </summary>
    public string ReplyToUserName { get; set; }

    /// <summary>
    /// 发起人 empId
    /// </summary>
    public string CreateUser { get; set; }

    /// <summary>
    /// 发起人姓名快照
    /// </summary>
    public string CreateUserName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public string TenantId { get; set; }

    /// <summary>
    /// 0 正常 1 已撤回
    /// </summary>
    public int IsDeleted { get; set; }
}
