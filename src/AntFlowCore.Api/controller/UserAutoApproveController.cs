using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;
using AntFlowCore.Business.service;
using AntFlowCore.Core.vo;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

/// <summary>
/// 用户自动审批设置. 对应 Java UserAutoApproveController.
/// </summary>
[Route("userAutoApprove")]
public class UserAutoApproveController
{
    private readonly UserAutoApproveService _userAutoApproveService;

    public UserAutoApproveController(UserAutoApproveService userAutoApproveService)
    {
        _userAutoApproveService = userAutoApproveService;
    }

    /// <summary>
    /// 分页列表(带活跃状态实时计算列)
    /// </summary>
    [HttpPost("listPage")]
    public ResultAndPage<UserAutoApproveVo> ListPage([FromBody] UserAutoApprovePageReq req)
    {
        return _userAutoApproveService.ListPage(req.PageDto, req.OwnerUserName, req.OwnerUserId, req.FormCode);
    }

    /// <summary>
    /// 活跃流程下拉(三类: DIY/LF/第三方)
    /// </summary>
    [HttpGet("activeConfList")]
    public Result<List<UserAutoApproveVo>> ActiveConfList()
    {
        return ResultHelper.Success(_userAutoApproveService.ActiveConfList());
    }

    /// <summary>
    /// 新增
    /// </summary>
    [HttpPost("save")]
    public Result<string> Save([FromBody] UserAutoApproveVo vo)
    {
        _userAutoApproveService.Save(vo);
        return ResultHelper.Success("ok");
    }

    /// <summary>
    /// 编辑
    /// </summary>
    [HttpPost("update")]
    public Result<string> Update([FromBody] UserAutoApproveVo vo)
    {
        _userAutoApproveService.Update(vo);
        return ResultHelper.Success("ok");
    }

    /// <summary>
    /// 启停
    /// </summary>
    [HttpPost("toggle/{id}/{enabled}")]
    public Result<string> Toggle([FromRoute] long id, [FromRoute] int enabled)
    {
        _userAutoApproveService.Toggle(id, enabled);
        return ResultHelper.Success("ok");
    }

    /// <summary>
    /// 删除(逻辑)
    /// </summary>
    [HttpGet("delete/{id}")]
    public Result<string> Delete([FromRoute] long id)
    {
        _userAutoApproveService.Delete(id);
        return ResultHelper.Success("ok");
    }

    /// <summary>
    /// 复制到最新活跃版本(含节点/表单校验)
    /// </summary>
    [HttpPost("copy/{id}")]
    public Result<string> Copy([FromRoute] long id)
    {
        _userAutoApproveService.Copy(id);
        return ResultHelper.Success("ok");
    }
}
