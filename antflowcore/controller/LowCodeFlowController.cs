using antflowcore.service.biz;
using antflowcore.vo;
using AntFlowCore.Entity;
using Microsoft.AspNetCore.Mvc;

namespace antflowcore.controller;

[Route("lowcode")]
public class LowCodeFlowController
{
    private readonly BpmnConfLFFormDataBizService _lfformDataBizService;
    private readonly DictService _dictService;

    public LowCodeFlowController(BpmnConfLFFormDataBizService lfformDataBizService, DictService dictService)
    {
        _lfformDataBizService = lfformDataBizService;
        _dictService = dictService;
    }

    [HttpPost("createLowCodeFormCode")]
    public Result<int> CreateLowCodeFormCode([FromBody] BaseKeyValueStruVo vo)
    {
        return Result<int>.Succ(_dictService.AddFormCode(vo));
    }
}