using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Business.service;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

/// <summary>
/// 流程千里眼 Controller
/// </summary>
[Route("flowClairvoyance")]
public class FlowClairvoyanceController
{
    private readonly FlowClairvoyanceService _flowClairvoyanceService;

    public FlowClairvoyanceController(FlowClairvoyanceService flowClairvoyanceService)
    {
        _flowClairvoyanceService = flowClairvoyanceService;
    }

    /// <summary>
    /// 分批搜索运行中流程的审批人
    /// </summary>
    [HttpPost("search")]
    public async Task<Result<FlowClairvoyanceResultVo>> Search([FromBody] FlowClairvoyanceVo vo)
    {
        if (vo.UserIds == null || vo.UserIds.Count == 0)
        {
            return ResultHelper.Fail<FlowClairvoyanceResultVo>("400", "请至少选择一个审批人", false, null);
        }

        if (vo.UserIds.Count > 5)
        {
            return ResultHelper.Fail<FlowClairvoyanceResultVo>("400", "最多选择5个审批人", false, null);
        }

        var result = await _flowClairvoyanceService.SearchAsync(vo);
        return ResultHelper.Success(result);
    }
}
