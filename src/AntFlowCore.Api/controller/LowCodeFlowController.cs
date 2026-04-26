using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.biz;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

[Route("lowcode")]
public class LowCodeFlowController
{
    private readonly IBpmnConfLFFormDataBizService _lfformDataBizService;
    private readonly IDicDataBizSerivce _dicDataBizSerivce;
    private readonly IDicDataSerivce _dicDataSerivce;
    private readonly IDictService _dictService;

    public LowCodeFlowController(IBpmnConfLFFormDataBizService lfformDataBizService,IDicDataBizSerivce dicDataBizSerivce,IDicDataSerivce dicDataSerivce)
    {
        _lfformDataBizService = lfformDataBizService;
        _dicDataBizSerivce = dicDataBizSerivce;
        _dicDataSerivce = dicDataSerivce;
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
        return _dicDataBizSerivce.SelectLFFormCodePageList(pageDto, taskMgmtVO);
    }
    [HttpPost("getLFActiveFormCodePageList")]
    public ResultAndPage<BaseKeyValueStruVo> GetLFActiveFormCodePageList([FromBody] DetailRequestDto requestDto)
    {
        var pageDto = requestDto.PageDto;
        var taskMgmtVO = requestDto.TaskMgmtVO;
        ResultAndPage<BaseKeyValueStruVo> resultAndPage = _dicDataBizSerivce.SelectLFActiveFormCodePageList(pageDto, taskMgmtVO);
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