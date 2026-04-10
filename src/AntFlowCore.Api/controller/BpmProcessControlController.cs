using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.vo;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

[Route("taskMgmt")]
public class BpmProcessControlController
{
    private readonly IProcessDeptBizService _processDeptService;

    public BpmProcessControlController(IProcessDeptBizService processDeptService)
    {
        _processDeptService = processDeptService;
    }
    [HttpPost("taskMgmt")]
    public Result<string> saveProcessNotices([FromBody] BpmProcessDeptVo vo) {
        _processDeptService.EditProcessConf(vo);
        return Result<string>.Succ("ok");
    }
    
}