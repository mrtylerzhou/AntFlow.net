using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

/// <summary>
/// 流程沟通接口.
/// 前端审批/查看表单页面, 通过 processNumber 拉取/发送该流程实例下的沟通消息.
/// </summary>
[Route("bpmnComment")]
public class BpmnCommentController
{
    private readonly IBpmProcessCommentService _processCommentService;

    public BpmnCommentController(IBpmProcessCommentService processCommentService)
    {
        _processCommentService = processCommentService;
    }

    /// <summary>
    /// 按 processNumber 查询未删除的沟通消息, 按 createTime + id 升序.
    /// </summary>
    [HttpGet("list")]
    public Result<List<BpmProcessComment>> List(string processNumber)
    {
        return ResultHelper.Success(_processCommentService.ListComments(processNumber));
    }

    /// <summary>
    /// 发送根消息或回复.
    /// </summary>
    [HttpPost("save")]
    public Result<BpmProcessComment> Save([FromBody] ProcessCommentVo vo)
    {
        return ResultHelper.Success(_processCommentService.AddComment(vo));
    }

    /// <summary>
    /// 撤回自己发送的消息.
    /// </summary>
    [HttpPost("withdraw")]
    public Result<object> Withdraw(long id)
    {
        _processCommentService.WithdrawComment(id);
        return ResultHelper.Success<object>(null);
    }
}
