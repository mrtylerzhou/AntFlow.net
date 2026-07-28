using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Business.service;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

/// <summary>
/// 流程效能统计 Controller
/// </summary>
[Route("processEfficiency")]
public class ProcessEfficiencyController
{
    private readonly ProcessEfficiencyService _processEfficiencyService;

    public ProcessEfficiencyController(ProcessEfficiencyService processEfficiencyService)
    {
        _processEfficiencyService = processEfficiencyService;
    }

    /// <summary>
    /// 触发效能统计计算
    /// </summary>
    [HttpPost("calculate")]
    public Result<string> Calculate([FromBody] ProcessEfficiencyVo vo)
    {
        List<string> formCodes = vo?.FormCodes;
        _processEfficiencyService.CalculateEfficiency(formCodes);
        return ResultHelper.Success("统计完成");
    }

    /// <summary>
    /// 分页查询流程级效能数据
    /// </summary>
    [HttpPost("page")]
    public Result<ResultAndPage<BpmProcessEfficiency>> Page([FromBody] ProcessEfficiencyVo vo)
    {
        var result = _processEfficiencyService.PageProcessLevel(vo);
        return ResultHelper.Success(result);
    }

    /// <summary>
    /// 查询节点级效能数据(展开流程行时调用)
    /// </summary>
    [HttpGet("nodes")]
    public Result<List<BpmProcessEfficiency>> Nodes([FromQuery] string procInstId)
    {
        var result = _processEfficiencyService.ListNodeLevel(procInstId);
        return ResultHelper.Success(result);
    }

    /// <summary>
    /// 查询任务级效能数据(展开节点行时调用)
    /// </summary>
    [HttpGet("tasks")]
    public Result<List<BpmProcessEfficiency>> Tasks([FromQuery] string procInstId, [FromQuery] string taskDefKey)
    {
        var result = _processEfficiencyService.ListTaskLevel(procInstId, taskDefKey);
        return ResultHelper.Success(result);
    }
}
