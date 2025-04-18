using antflowcore.dto;
using AntFlowCore.Entity;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Vo;
using Microsoft.AspNetCore.Mvc;

namespace antflowcore.controller;

[Route("lowcode")]
public class LowCodeFlowController
{
    private readonly BpmnConfLFFormDataBizService _lfformDataBizService;
    private readonly DictService _dictService;

    public LowCodeFlowController(BpmnConfLFFormDataBizService lfformDataBizService,DictService dictService)
    {
        _lfformDataBizService = lfformDataBizService;
        _dictService = dictService;
    }
    [HttpPost("createLowCodeFormCode")]
    public Result<int> CreateLowCodeFormCode([FromBody] BaseKeyValueStruVo vo){
        return Result<int>.Succ(_dictService.AddFormCode(vo));
    }
    
    [HttpGet("getLowCodeFlowFormCodes")]
    public Result<List<BaseKeyValueStruVo>> GetLowCodeFormCodes()
    {
        List<BaseKeyValueStruVo> lowCodeFlowFormCodes = _dictService.GetLowCodeFlowFormCodes();
        return ResultHelper.Success(lowCodeFlowFormCodes);
    }
    [HttpPost("getLFFormCodePageList")]
    public ResultAndPage<BaseKeyValueStruVo> GetLFFormCodePageList(DetailRequestDto requestDto) {
        PageDto pageDto = requestDto.PageDto;
        TaskMgmtVO taskMgmtVO = requestDto.TaskMgmtVO;
        return _dictService.SelectLFFormCodePageList(pageDto, taskMgmtVO);
    }
}