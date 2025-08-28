using AntFlow.Core.Entity;
using AntFlow.Core.Service.Business;
using AntFlow.Core.Vo;
using Microsoft.AspNetCore.Mvc;

namespace AntFlow.Core.Controller;

[Route("taskMgmt")]
public class BpmProcessControlController
{
    private readonly ProcessDeptBizService _processDeptService;

    public BpmProcessControlController(ProcessDeptBizService processDeptService)
    {
        _processDeptService = processDeptService;
    }

    [HttpPost("taskMgmt")]
    public Result<string> saveProcessNotices([FromBody] BpmProcessDeptVo vo)
    {
        _processDeptService.EditProcessConf(vo);
        return Result<string>.Succ("ok");
    }
}