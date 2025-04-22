using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Entity;
using AntFlowCore.Vo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace antflowcore.controller;

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
    /// 根据表单编码查询回调配置列表
    /// </summary>
    [HttpGet("callbackUrlConf/list/{formCode}")]
    public Result<List<OutSideBpmCallbackUrlConf>> List(string formCode)
    {
        var result = _outSideBpmCallbackUrlConfService.SelectListByFormCode(formCode);
        return ResultHelper.Success(result);
    }

    /// <summary>
    /// 查询指定回调配置详情
    /// </summary>
    [HttpGet("callbackUrlConf/detail/{id}")]
    public Result<OutSideBpmCallbackUrlConfVo> Detail(int id)
    {
        var result = _outSideBpmCallbackUrlConfService.Detail(id);
        return ResultHelper.Success(result);
    }

    /// <summary>
    /// 编辑回调配置
    /// </summary>
    [HttpPost("callbackUrlConf/edit")]
    public Result<string> Edit([FromBody] OutSideBpmCallbackUrlConfVo vo)
    {
        _outSideBpmCallbackUrlConfService.Edit(vo);
        return ResultHelper.Success("ok");
    }
}