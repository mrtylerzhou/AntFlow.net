using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

/// <summary>
/// Process control controller (equivalent to Java BpmProcessControlController).
/// Provides task management configuration endpoints like saving process notices,
/// retrieving form-related options, and UDR options.
/// </summary>
[Route("taskMgmt")]
public class ProcessControlController
{
    private readonly IBpmnConfBizService _bpmnConfBizService;
    private readonly IDicDataSerivce _dicDataService;

    public ProcessControlController(
        IBpmnConfBizService bpmnConfBizService,
        IDicDataSerivce dicDataService)
    {
        _bpmnConfBizService = bpmnConfBizService;
        _dicDataService = dicDataService;
    }

    /// <summary>
    /// Save process notices configuration.
    /// Under the process icon there is a configuration option to save process permissions
    /// (not yet implemented), process notification types, and advanced notification templates.
    /// </summary>
    /// <param name="vo">process configuration vo</param>
    /// <returns></returns>
    [HttpPost("taskMgmt")]
    public Result<object> SaveProcessNotices([FromBody] ProcessConfVo vo)
    {
        _bpmnConfBizService.SaveProcessNotices(vo);
        return ResultHelper.Success<object>(null!);
    }
    
}
