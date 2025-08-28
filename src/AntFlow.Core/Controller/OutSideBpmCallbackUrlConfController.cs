using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using Microsoft.AspNetCore.Mvc;

namespace AntFlow.Core.Controller;

[Route("outSideBpm")]
public class OutSideBpmCallbackUrlConfController
{
    private readonly ILogger<OutSideBpmCallbackUrlConfController> _logger;
    private readonly OutSideBpmCallbackUrlConfService _outSideBpmCallbackUrlConfService;

    public OutSideBpmCallbackUrlConfController(
        ILogger<OutSideBpmCallbackUrlConfController> logger,
        OutSideBpmCallbackUrlConfService outSideBpmCallbackUrlConfService)
    {
        _logger = logger;
        _outSideBpmCallbackUrlConfService = outSideBpmCallbackUrlConfService;
    }

    /// <summary>
    ///     ????????????????????งา?
    /// </summary>
    [HttpGet("callbackUrlConf/list/{formCode}")]
    public Result<List<OutSideBpmCallbackUrlConf>> List(string formCode)
    {
        List<OutSideBpmCallbackUrlConf>? result = _outSideBpmCallbackUrlConfService.SelectListByFormCode(formCode);
        return ResultHelper.Success(result);
    }


    /// <summary>
    ///     ?????????????????
    /// </summary>
    [HttpGet("callbackUrlConf/detail/{id}")]
    public Result<OutSideBpmCallbackUrlConfVo> Detail(int id)
    {
        OutSideBpmCallbackUrlConfVo? result = _outSideBpmCallbackUrlConfService.Detail(id);
        return ResultHelper.Success(result);
    }

    /// <summary>
    ///     ?????????
    /// </summary>
    [HttpPost("callbackUrlConf/edit")]
    public Result<string> Edit([FromBody] OutSideBpmCallbackUrlConfVo vo)
    {
        _outSideBpmCallbackUrlConfService.Edit(vo);
        return ResultHelper.Success("ok");
    }
}