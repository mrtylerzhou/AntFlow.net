using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

/// <summary>
/// 发起流程页(任务中心). 对应 Java StartFlowListController.
/// </summary>
[Route("startFlowList")]
public class StartFlowListController
{
    private readonly IStartFlowListBizService _startFlowListBizService;

    public StartFlowListController(IStartFlowListBizService startFlowListBizService)
    {
        _startFlowListBizService = startFlowListBizService;
    }

    /// <summary>
    /// 发起流程分页(页 = 最多 3 栏,栏内按分类块)
    /// </summary>
    [HttpPost("page")]
    public ResultAndPage<StartFlowCategoryVo> Page([FromBody] StartFlowListPageReq? req)
    {
        return _startFlowListBizService.Page(req);
    }
}
