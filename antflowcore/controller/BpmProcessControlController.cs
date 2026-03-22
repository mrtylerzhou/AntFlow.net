using AntFlowCore.Constant.Enums;
using AntFlowCore.Entity;
using antflowcore.service.biz;
using antflowcore.vo;
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
    public Result<string> SaveProcessNotices([FromBody] BpmProcessDeptVo vo)
    {
        _processDeptService.EditProcessConf(vo);
        return Result<string>.Succ("ok");
    }

    /// <summary>
    /// 获取表单关联选项
    /// </summary>
    [HttpGet("getFormRelatedOptions")]
    public Result<List<BaseNumIdStruVo>> GetFormRelatedOptions()
    {
        var list = new List<BaseNumIdStruVo>();
        foreach (var value in NodeFormAssigneeProperty.AllValues)
        {
            list.Add(new BaseNumIdStruVo(value.Code, value.Desc,true));
        }
        return Result<List<BaseNumIdStruVo>>.Succ(list);
    }
}