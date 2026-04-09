using AntFlowCore.Abstraction;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;
using AntFlowCore.Core;
using AntFlowCore.Core.vo;
using AntFlowCore.Extensions;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Api.controller;

[Route("outSide")]
public class OutSideBpmAccessController
{
    private readonly IOutSideBpmAccessBusinessService _outSideBpmAccessBusinessService;
    private readonly ILogger<OutSideBpmAccessController> _logger;

    public OutSideBpmAccessController(
        IOutSideBpmAccessBusinessService outSideBpmAccessBusinessService,
        ILogger<OutSideBpmAccessController> logger)
    {
        _outSideBpmAccessBusinessService = outSideBpmAccessBusinessService;
        _logger = logger;
    }

    /// <summary>
    /// 外部工作流提交
    /// </summary>
    /// <param name="vo"></param>
    /// <returns></returns>
    [HttpPost("processSubmit")]
    public Result<OutSideBpmAccessRespVo> AccessBusinessStart([FromBody] OutSideBpmAccessBusinessVo vo)
    {
        OutSideBpmAccessRespVo data = _outSideBpmAccessBusinessService.AccessBusinessStart(vo);
        return ResultHelper.Success(data);
    }

    /// <summary>
    /// 分页获取外部工作流的formcode(外部的都是通过字典配置的,内嵌的是通过实现IFormOperationAdaptor的服务上的AfFormServiceAnno属性拿到的
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("getOutSideFormCodePageList")]
    public ResultAndPage<BpmnConfVo> ListPage([FromBody] PageRequestDto<BpmnConfVo> dto)
    {
        return _outSideBpmAccessBusinessService.SelectOutSideFormCodePageList(dto.PageDto, dto.Entity);
    }

    /*[HttpPost("processPreview")]
    public Result<object> AccessBusinessPreview([FromBody] OutSideBpmAccessBusinessVo vo)
    {
        var preview = _outSideBpmAccessBusinessService.AccessBusinessPreview(vo);
        return Result<object>.NewSuccessResult(preview);
    }*/

    [HttpPost("processBreak")]
    public Result<string> AccessBusinessBreak([FromBody] OutSideBpmAccessBusinessVo vo)
    {
        _outSideBpmAccessBusinessService.ProcessBreak(vo);
        return ResultHelper.Success("ok");
    }

    [HttpGet("outSideProcessRecord")]
    public Result<List<OutSideBpmAccessProcessRecordVo>> OutSideProcessRecord([FromQuery] string processNumber)
    {
        List<OutSideBpmAccessProcessRecordVo> records = _outSideBpmAccessBusinessService.OutSideProcessRecord(processNumber);
        return ResultHelper.Success(records);
    }
}