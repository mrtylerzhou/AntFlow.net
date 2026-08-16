using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

/// <summary>
/// 流程分类管理(PC 端). 对应 Java BpmProcessCategoryController.
/// 管理 bpm_process_category 表数据; 下拉选项供流程设计器「流程类型」使用.
/// </summary>
[Route("processCategory")]
public class ProcessCategoryController
{
    private readonly IProcessCategoryService _processCategoryService;

    public ProcessCategoryController(IProcessCategoryService processCategoryService)
    {
        _processCategoryService = processCategoryService;
    }

    /// <summary>
    /// 分页列表
    /// </summary>
    [HttpPost("listPage")]
    public ResultAndPage<BpmProcessCategoryVo> ListPage([FromBody] BpmProcessCategoryPageReq req)
    {
        BpmProcessCategoryVo vo = new() { ProcessTypeName = req?.ProcessTypeName };
        return _processCategoryService.SelectPage(req?.PageDto ?? PageDto.First(), vo);
    }

    /// <summary>
    /// 新增/编辑分类
    /// </summary>
    [HttpPost("save")]
    public Result<string> Save([FromBody] BpmProcessCategoryVo vo)
    {
        try
        {
            vo.IsApp ??= 0;
            _processCategoryService.EditProcessCategory(vo);
            return ResultHelper.Success("ok");
        }
        catch (AFBizException e)
        {
            return ResultHelper.Fail<string>(e.Code, e.Msg, false, e);
        }
    }

    /// <summary>
    /// 分类操作: 2 上移 / 3 下移 / 4 删除
    /// </summary>
    [HttpGet("operation/{type}/{id}")]
    public Result<string> Operation([FromRoute] int type, [FromRoute] long id)
    {
        try
        {
            _processCategoryService.CategoryOperation(type, id);
            return ResultHelper.Success("ok");
        }
        catch (AFBizException e)
        {
            return ResultHelper.Fail<string>(e.Code, e.Msg, false, e);
        }
    }

    /// <summary>
    /// 下拉选项(流程设计器-基础设置-流程类型): is_del=0, 不过滤内置 id、不过滤 is_app
    /// </summary>
    [HttpGet("options")]
    public Result<List<BpmProcessCategoryVo>> Options()
    {
        return ResultHelper.Success(_processCategoryService.Options());
    }
}
