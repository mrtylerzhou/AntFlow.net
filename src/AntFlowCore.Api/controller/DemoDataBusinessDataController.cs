using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

/// <summary>
/// 演示数据-业务数据 动态列表. 对应 Java DemoDataBusinessDataController.
/// 接口路径与 Java 版保持一致(前端共享).
/// </summary>
[Route("demoData/businessData")]
public class DemoDataBusinessDataController
{
    private readonly IDemoDataBusinessDataService _demoDataBusinessDataService;

    public DemoDataBusinessDataController(IDemoDataBusinessDataService demoDataBusinessDataService)
    {
        _demoDataBusinessDataService = demoDataBusinessDataService;
    }

    /// <summary>
    /// 分页列表(columns + rows + total)
    /// </summary>
    [HttpPost("listPage")]
    public Result<BusinessDataListVo> ListPage([FromBody] BusinessDataListPageReq req)
    {
        return ResultHelper.Success(_demoDataBusinessDataService.ListPage(req));
    }

    /// <summary>
    /// 流程详情查看权限校验
    /// </summary>
    [HttpPost("checkPermission")]
    public Result<bool> CheckPermission([FromQuery] string processNumber)
    {
        return ResultHelper.Success(_demoDataBusinessDataService.CheckPermission(processNumber));
    }
}