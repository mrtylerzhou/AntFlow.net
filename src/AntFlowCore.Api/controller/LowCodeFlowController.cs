using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.constant.enums;
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
    private readonly ILfFormManageBizService _lfFormManageBizService;
    private readonly IBpmnConfBizService _bpmnConfBizService;

    public LowCodeFlowController(
        IBpmnConfLFFormDataBizService lfformDataBizService,
        IDicDataBizSerivce dicDataBizSerivce,
        IDictService dicSerivce,
        ILfFormManageBizService lfFormManageBizService,
        IBpmnConfBizService bpmnConfBizService)
    {
        _lfformDataBizService = lfformDataBizService;
        _dicDataBizSerivce = dicDataBizSerivce;
        _dictService = dicSerivce;
        _lfFormManageBizService = lfFormManageBizService;
        _bpmnConfBizService = bpmnConfBizService;
    }

    [HttpPost("createLowCodeFormCode")]
    public Result<int> CreateLowCodeFormCode([FromBody] BaseKeyValueStruVo vo)
    {
        return Result<int>.Succ(_dictService.AddFormCode(vo));
    }

    [HttpGet("getLowCodeFlowFormCodes")]
    public Result<List<BaseKeyValueStruVo>> GetLowCodeFormCodes()
    {
        List<BaseKeyValueStruVo> lowCodeFlowFormCodes = _dictService.GetLowCodeFlowFormCodes();
        return ResultHelper.Success(lowCodeFlowFormCodes);
    }

    [HttpPost("getLFFormCodePageList")]
    public ResultAndPage<BaseKeyValueStruVo> GetLFFormCodePageList([FromBody] DetailRequestDto requestDto)
    {
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

    /// <summary>
    /// 发起流程页: 根据 formCode 获取表单数据(兼容内联/外部表单模式)
    /// 内联模式返回 useExternalForm=false + lfFormData
    /// 外部模式返回 useExternalForm=true + lfFormdataList
    /// </summary>
    [HttpGet("getStartFormData")]
    public Result<LfStartFormVo> GetStartFormData(string formCode)
    {
        if (string.IsNullOrEmpty(formCode))
        {
            throw new AFBizException("请传入formcode");
        }
        BpmnConfVo confVo = _bpmnConfBizService.DetailByFormCode(formCode);
        var result = new LfStartFormVo();
        bool useExternal = BpmnConfFlagsEnum.HasFlag(confVo.ExtraFlags, BpmnConfFlagsEnum.USE_EXTERNAL_FORM);
        result.UseExternalForm = useExternal;
        if (useExternal)
        {
            result.LfFormdataList = confVo.LfFormdataList;
        }
        else
        {
            result.LfFormData = confVo.LfFormData;
        }
        return ResultHelper.Success(result);
    }

    // ===================== 独立表单管理 =====================

    /// <summary>
    /// 分页查询独立表单（家族分组，每族一行生效版本）
    /// </summary>
    [HttpPost("form/listPage")]
    public ResultAndPage<LfFormManageVo> ListFormPage([FromBody] DetailRequestDto requestDto)
    {
        PageDto pageDto = requestDto.PageDto;
        var vo = new LfFormManageVo();
        if (requestDto.TaskMgmtVO != null)
        {
            vo.Search = requestDto.TaskMgmtVO.Search;
        }
        return _lfFormManageBizService.ListPage(pageDto, vo);
    }

    /// <summary>
    /// 按 id 查询表单版本（编辑回显 / 审批按 id 取 formdata）
    /// </summary>
    [HttpGet("form/{id}")]
    public Result<LfFormManageVo> GetFormById(long id)
    {
        return ResultHelper.Success(_lfFormManageBizService.GetById(id));
    }

    /// <summary>
    /// 保存表单：无 formCode => 新建家族+首版本；有 formCode => 新建版本
    /// </summary>
    [HttpPost("form/save")]
    public Result<long> SaveForm([FromBody] LfFormManageVo vo)
    {
        return ResultHelper.Success(_lfFormManageBizService.Save(vo));
    }

    /// <summary>
    /// 软删除单个版本（被生效流程引用时拒绝）
    /// </summary>
    [HttpDelete("form/{id}")]
    public Result<object> DeleteForm(long id)
    {
        _lfFormManageBizService.Delete(id);
        return ResultHelper.Success<object>(null);
    }

    /// <summary>
    /// 查询某家族所有版本（历史版本查看）
    /// </summary>
    [HttpGet("form/history")]
    public Result<List<LfFormManageVo>> ListFormHistory(string formCode)
    {
        if (string.IsNullOrEmpty(formCode))
        {
            throw new AFBizException("请传入formCode");
        }
        return ResultHelper.Success(_lfFormManageBizService.ListHistory(formCode));
    }

    /// <summary>
    /// 列出所有生效独立表单（流程设计多选下拉框）
    /// </summary>
    [HttpGet("form/listEffectiveForSelect")]
    public Result<List<LfFormManageVo>> ListEffectiveForSelect()
    {
        return ResultHelper.Success(_lfFormManageBizService.ListEffectiveForSelect());
    }

    /// <summary>
    /// 生效指定表单版本（同族其他版本自动置为非生效）
    /// </summary>
    [HttpPut("form/effective/{id}")]
    public Result<object> Effective(long id)
    {
        _lfFormManageBizService.Effective(id);
        return ResultHelper.Success<object>(null);
    }
}
