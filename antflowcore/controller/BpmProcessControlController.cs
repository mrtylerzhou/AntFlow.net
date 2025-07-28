using AntFlowCore.Entity;
using antflowcore.service.biz;
using AntFlowCore.Vo;
using Microsoft.AspNetCore.Mvc;

namespace antflowcore.controller;

[Route("taskMgmt")]
public class BpmProcessControlController
{
    private readonly ProcessDeptBizService _processDeptService;

    public BpmProcessControlController(ProcessDeptBizService processDeptService)
    {
        _processDeptService = processDeptService;
    }
    [HttpPost("taskMgmt")]
    public Result<string> saveProcessNotices([FromBody] BpmProcessDeptVo vo) {
        _processDeptService.EditProcessConf(vo);
        return Result<string>.Succ("ok");
    }
    
}