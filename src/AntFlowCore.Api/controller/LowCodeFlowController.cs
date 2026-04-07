using AntFlowCore.Abstraction;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Common.exception;
using AntFlowCore.Common.util;
using AntFlowCore.Core;
using AntFlowCore.Core.dto;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.vo;
using AntFlowCore.Engine.service.biz;
using AntFlowCore.Engine.service.repository;
using AntFlowCore.Entity;
using AntFlowCore.Extensions;
using AntFlowCore.Vo;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

[Route("lowcode")]
public class LowCodeFlowController
{
    private readonly IBpmnConfLFFormDataBizService _lfformDataBizService;
    private readonly IDictService _dictService;

    public LowCodeFlowController(IBpmnConfLFFormDataBizService lfformDataBizService,IDictService dictService)
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
    public ResultAndPage<BaseKeyValueStruVo> GetLFFormCodePageList([FromBody] DetailRequestDto requestDto) {
        PageDto pageDto = requestDto.PageDto;
        TaskMgmtVO taskMgmtVO = requestDto.TaskMgmtVO;
        return _dictService.SelectLFFormCodePageList(pageDto, taskMgmtVO);
    }
    [HttpPost("getLFActiveFormCodePageList")]
    public ResultAndPage<BaseKeyValueStruVo> GetLFActiveFormCodePageList([FromBody] DetailRequestDto requestDto)
    {
        var pageDto = requestDto.PageDto;
        var taskMgmtVO = requestDto.TaskMgmtVO;
        ResultAndPage<BaseKeyValueStruVo> resultAndPage = _dictService.SelectLFActiveFormCodePageList(pageDto, taskMgmtVO);
        return resultAndPage;
    }
    [HttpGet("getformDataByFormCode")]
    public Result<string> GetLFFormDataByFormCode(string formCode)
    {
        if (string.IsNullOrEmpty(formCode))
        {
            throw new AFBizException("请传入formcode"); 
        }
        BpmnConfLfFormdata lfFormDataByFormCode = _lfformDataBizService.GetLFFormDataByFormCode(formCode);
        return ResultHelper.Success(lfFormDataByFormCode.Formdata);
    }
}