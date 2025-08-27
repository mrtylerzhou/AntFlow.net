using AntFlow.Core.Dto;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using Microsoft.AspNetCore.Mvc;

namespace AntFlow.Core.Controller;

[Route("outSide")]
public class OutSideBpmAccessController
{
    private readonly ILogger<OutSideBpmAccessController> _logger;
    private readonly OutSideBpmAccessBusinessService _outSideBpmAccessBusinessService;

    public OutSideBpmAccessController(
        OutSideBpmAccessBusinessService outSideBpmAccessBusinessService,
        ILogger<OutSideBpmAccessController> logger)
    {
        _outSideBpmAccessBusinessService = outSideBpmAccessBusinessService;
        _logger = logger;
    }

    /// <summary>
    ///     ??????????
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
    ///     ????????????????formcode(??????????????????,?????????????IFormOperationAdaptor????????AfFormServiceAnno?????????
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
        List<OutSideBpmAccessProcessRecordVo> records =
            _outSideBpmAccessBusinessService.OutSideProcessRecord(processNumber);
        return ResultHelper.Success(records);
    }
}