namespace AntFlowCore.Base.vo;

/// <summary>
/// 流程沟通 发送消息入参 VO.
/// </summary>
public class ProcessCommentVo
{
    /// <summary>
    /// 流程实例编号(会话锚点)
    /// </summary>
    public string ProcessNumber { get; set; }

    /// <summary>
    /// 回复哪条消息(根消息为 null)
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// 消息正文
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 图片/附件 url JSON 数组(仅预留, v1 不处理)
    /// </summary>
    public string Attachment { get; set; }

    /// <summary>
    /// @提及列表
    /// </summary>
    public List<Mention> Mentions { get; set; }

    public class Mention
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
