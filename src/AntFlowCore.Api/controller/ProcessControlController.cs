using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

/// <summary>
/// Provides task management configuration endpoints like saving process notices,
/// retrieving form-related options, and UDR options.
/// </summary>
[Route("taskMgmt")]
public class ProcessControlController
{
    private readonly IBpmnConfBizService _bpmnConfBizService;
    private readonly IDicDataSerivce _dicDataService;

    public ProcessControlController(
        IBpmnConfBizService bpmnConfBizService,
        IDicDataSerivce dicDataService)
    {
        _bpmnConfBizService = bpmnConfBizService;
        _dicDataService = dicDataService;
    }

    /// <summary>
    /// Save process notices configuration.
    /// Under the process icon there is a configuration option to save process permissions
    /// (not yet implemented), process notification types, and advanced notification templates.
    /// </summary>
    /// <param name="vo">process configuration vo</param>
    /// <returns></returns>
    [HttpPost("taskMgmt")]
    public Result<object> SaveProcessNotices([FromBody] ProcessConfVo vo)
    {
        _bpmnConfBizService.SaveProcessNotices(vo);
        return ResultHelper.Success<object>(null!);
    }

    /// <summary>
    /// Returns the list of form-related assignee property type options
    /// (e.g. 表单中的人员, 表单中的角色, 表单中人员的直属领导, etc.).
    /// Used by the frontend when configuring a "从表单中选取" approver node.
    /// </summary>
    [HttpGet("getFormRelatedOptions")]
    public Result<List<BaseNumIdStruVo>> GetFormRelatedOptions()
    {
        var list = new List<BaseNumIdStruVo>();
        foreach (NodeFormAssigneePropertyEnum value in Enum.GetValues(typeof(NodeFormAssigneePropertyEnum)))
        {
            list.Add(new BaseNumIdStruVo((int)value, NodeFormAssigneePropertyEnumExtensions.GetDescByCode((int)value)));
        }
        return ResultHelper.Success(list);
    }

    /// <summary>
    /// Returns the list of user-defined-rule (UDR) assignee type options
    /// (e.g. 自定义审批人1, 自定义审批人2, etc.).
    /// Sourced from the dictionary where dict_type = "udr".
    /// Used by the frontend when configuring a "自定义规则" approver node.
    /// </summary>
    [HttpGet("getUDROptions")]
    public Result<List<BaseIdTranStruVo>> GetUDROptions()
    {
        List<DictData> dictDataList = _dicDataService._repository
            .Find(a => a.DictType == AFSpecialDictCategoryEnumExtensions.USER_DEFINED_RULE_FOR_ASSIGNEE)
            .ToList();

        var list = dictDataList
            .Select(a => new BaseIdTranStruVo(a.Value, a.Label))
            .ToList();

        return ResultHelper.Success(list);
    }
    
}
