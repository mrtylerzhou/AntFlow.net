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
    private readonly IDictService _dictService;

    public LowCodeFlowController(IBpmnConfLFFormDataBizService lfformDataBizService,IDicDataBizSerivce dicDataBizSerivce,IDictService dicSerivce)
    {
        _lfformDataBizService = lfformDataBizService;
        _dicDataBizSerivce = dicDataBizSerivce;
        _dictService = dicSerivce;
    }
    [HttpPost("createLowCodeFormCode")]
    public Result<int> CreateLowCodeFormCode([FromBody] BaseKeyValueStruVo vo){
        return Result<int>.Succ(_dictService.AddFormCode(vo));
    }

    /// <summary>
    /// 新增 page-added DIY FormCode(dict_type=diylowcodeflow: LF 后端 + 自定义 Vue 前端)
    /// </summary>
    [HttpPost("createDIYFormCode")]
    public Result<int> CreateDIYFormCode([FromBody] BaseKeyValueStruVo vo)
    {
        return Result<int>.Succ(_dictService.AddDIYFormCode(vo));
    }

    /// <summary>
    /// 获取 page-added DIY FormCode Page List 模板列表使用
    /// </summary>
    [HttpPost("getDIYFormCodePageList")]
    public ResultAndPage<BaseKeyValueStruVo> GetDIYFormCodePageList([FromBody] DetailRequestDto requestDto)
    {
        PageDto pageDto = requestDto.PageDto;
        TaskMgmtVO taskMgmtVO = requestDto.TaskMgmtVO;
        return _dicDataBizSerivce.SelectDIYFormCodePageList(pageDto, taskMgmtVO);
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