using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;

namespace AntFlowCore.Persist.api.interf.repository;

public interface IBpmProcessCommentService : IAntFlowRepositoryMix<BpmProcessComment, IBpmProcessCommentRepository>
{
    /// <summary>
    /// 按 processNumber 查询未删除的沟通消息, 按 createTime + id 升序.
    /// </summary>
    List<BpmProcessComment> ListComments(string processNumber);

    /// <summary>
    /// 发送根消息或回复(二级回复, @提及发站内信).
    /// </summary>
    BpmProcessComment AddComment(ProcessCommentVo vo);

    /// <summary>
    /// 撤回自己发送的消息(软删除 is_deleted=1).
    /// </summary>
    void WithdrawComment(long id);
}
