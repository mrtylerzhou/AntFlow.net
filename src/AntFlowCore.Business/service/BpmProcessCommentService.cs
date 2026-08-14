using System.Text.Json;
using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Business.service;

/// <summary>
/// 流程沟通.
/// 对齐 Java 版 jimuoffice 的 ProcessCommentBizServiceImpl:
/// - 按 processNumber 一条会话, 二级回复(parentId/rootId 扁平存);
/// - @提及发站内信(IMessageService);
/// - 撤回走软删除(is_deleted=1), 仅本人可撤回.
/// </summary>
public class BpmProcessCommentService : IBpmProcessCommentService
{
    private readonly IMessageService _messageService;
    private readonly ILogger<BpmProcessCommentService> _logger;

    public BpmProcessCommentService(
        IBpmProcessCommentRepository repository,
        IMessageService messageService,
        ILogger<BpmProcessCommentService> logger)
    {
        _repository = repository;
        _messageService = messageService;
        _logger = logger;
    }

    public IBpmProcessCommentRepository _repository { get; }

    public List<BpmProcessComment> ListComments(string processNumber)
    {
        if (string.IsNullOrEmpty(processNumber))
        {
            return new List<BpmProcessComment>();
        }
        return _repository.GetQueryable()
            .Where(c => c.ProcessNumber == processNumber && c.IsDeleted == 0)
            .OrderBy(c => c.CreateTime)
            .ThenBy(c => c.Id)
            .ToList();
    }

    public BpmProcessComment AddComment(ProcessCommentVo vo)
    {
        if (vo == null || string.IsNullOrEmpty(vo.ProcessNumber))
        {
            throw new ArgumentException("processNumber 不能为空");
        }

        var comment = new BpmProcessComment
        {
            ProcessNumber = vo.ProcessNumber,
            Content = vo.Content,
            Attachment = vo.Attachment,
            CreateUser = SecurityUtils.GetLogInEmpIdStr(),
            CreateUserName = SecurityUtils.GetLogInEmpNameSafe(),
            CreateTime = DateTime.Now,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
            IsDeleted = 0,
        };

        // 二级回复模型: 根消息 parentId/rootId 均 null; 回复消息挂到根, 并记录回给谁
        long? parentId = vo.ParentId;
        if (parentId != null)
        {
            BpmProcessComment parent = _repository.GetById(parentId.Value);
            if (parent == null)
            {
                throw new ArgumentException("被回复的消息不存在或已删除");
            }
            comment.ParentId = parentId;
            // root: 父是根 → 父自身; 父是回复 → 父的 root
            comment.RootId = parent.ParentId == null ? parent.Id : parent.RootId;
            comment.ReplyToUser = parent.CreateUser;
            comment.ReplyToUserName = parent.CreateUserName;
        }

        // @提及序列化
        if (vo.Mentions != null && vo.Mentions.Count > 0)
        {
            comment.Mentions = JsonSerializer.Serialize(vo.Mentions);
        }

        _repository.Add(comment);
        _repository.SaveChanges();

        // @提及发站内信
        if (vo.Mentions != null && vo.Mentions.Count > 0)
        {
            NotifyMentions(comment, vo);
        }
        return comment;
    }

    public void WithdrawComment(long id)
    {
        BpmProcessComment comment = _repository.GetById(id);
        if (comment == null)
        {
            throw new ArgumentException("消息不存在");
        }
        string loginId = SecurityUtils.GetLogInEmpIdStr();
        if (comment.CreateUser != loginId)
        {
            throw new ArgumentException("只能撤回自己发送的消息");
        }
        comment.IsDeleted = 1;
        _repository.Update(comment);
        _repository.SaveChanges();
    }

    private void NotifyMentions(BpmProcessComment comment, ProcessCommentVo vo)
    {
        string fromName = string.IsNullOrEmpty(comment.CreateUserName)
            ? comment.CreateUser : comment.CreateUserName;
        string snippet = comment.Content;
        if (!string.IsNullOrEmpty(snippet) && snippet.Length > 50)
        {
            snippet = snippet.Substring(0, 50) + "...";
        }
        foreach (ProcessCommentVo.Mention m in vo.Mentions)
        {
            if (m == null || string.IsNullOrEmpty(m.UserId))
            {
                continue;
            }
            try
            {
                _messageService.InsertUserMessage(new UserMessage
                {
                    UserId = m.UserId,
                    Title = "流程沟通提醒",
                    Content = $"{fromName} 在流程 {comment.ProcessNumber} 的沟通中@了你：{snippet}",
                    IsRead = false,
                    Source = 0,
                    CreateTime = DateTime.Now,
                });
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "notify mention failed, userId={UserId}", m.UserId);
            }
        }
    }
}
