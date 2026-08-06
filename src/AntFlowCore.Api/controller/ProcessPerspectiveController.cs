using AntFlowCore.Abstraction.service;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.vo;
using AntFlowCore.Business.service;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

/// <summary>
/// 流程透视 Controller
/// </summary>
[Route("processPerspective")]
public class ProcessPerspectiveController
{
    private readonly ProcessPerspectiveService _processPerspectiveService;
    private readonly ITaskMgmtService _taskMgmtService;
    private readonly IDictService _dictService;
    private readonly IOutSideBpmAccessBusinessService _outSideBpmAccessBusinessService;

    public ProcessPerspectiveController(
        ProcessPerspectiveService processPerspectiveService,
        ITaskMgmtService taskMgmtService,
        IDictService dictService,
        IOutSideBpmAccessBusinessService outSideBpmAccessBusinessService)
    {
        _processPerspectiveService = processPerspectiveService;
        _taskMgmtService = taskMgmtService;
        _dictService = dictService;
        _outSideBpmAccessBusinessService = outSideBpmAccessBusinessService;
    }

    /// <summary>
    /// 获取全部流程列表(DIY/LF/SaaS合并)
    /// </summary>
    [HttpPost("getAllFormCodeList")]
    public Result<List<DIYProcessInfoDTO>> GetAllFormCodeList([FromQuery] string desc)
    {
        var result = new List<DIYProcessInfoDTO>();

        // DIY流程
        var diyList = _taskMgmtService.ViewProcessInfo(desc ?? "");
        if (diyList != null)
        {
            foreach (var item in diyList)
            {
                item.Type = "DIY";
                result.Add(item);
            }
        }

        // LF低代码流程
        var lfList = _dictService.GetLowCodeFlowFormCodes();
        if (lfList != null)
        {
            foreach (var item in lfList)
            {
                result.Add(new DIYProcessInfoDTO
                {
                    Key = item.Key,
                    Value = item.Value,
                    Type = "LF",
                    Remark = item.Remark
                });
            }
        }

        // 第三方流程
        var outsideResult = _outSideBpmAccessBusinessService.SelectOutSideFormCodePageList(
            new PageDto { Page = 1, PageSize = 9999 }, new BpmnConfVo());
        if (outsideResult?.Data != null)
        {
            foreach (var item in outsideResult.Data)
            {
                result.Add(new DIYProcessInfoDTO
                {
                    Key = item.FormCode,
                    Value = item.BpmnName ?? item.FormCode,
                    Type = "OUTSIDE"
                });
            }
        }

        // 按desc过滤
        if (!string.IsNullOrWhiteSpace(desc))
        {
            string lowerDesc = desc.ToLower();
            result = result.Where(r =>
                (r.Value != null && r.Value.ToLower().Contains(lowerDesc))
                || (r.Key != null && r.Key.ToLower().Contains(lowerDesc))
            ).ToList();
        }

        return ResultHelper.Success(result);
    }

    /// <summary>
    /// 分批搜索流程配置
    /// </summary>
    [HttpPost("search")]
    public Result<ProcessPerspectiveResultVo> Search([FromBody] ProcessPerspectiveVo vo)
    {
        return ResultHelper.Success(_processPerspectiveService.Search(vo));
    }
}
