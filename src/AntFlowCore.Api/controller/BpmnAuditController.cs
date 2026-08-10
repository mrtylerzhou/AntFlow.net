using AntFlowCore.Abstraction.util;
using AntFlowCore.Base.entity;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.AspNetCore.Mvc;

namespace AntFlowCore.Api.controller;

/// <summary>
/// 流程表单字段变更审计查询接口.
/// 前端审批/查看表单页面, 通过 processNumber 拉取该流程实例下所有节点的字段变更记录.
/// </summary>
[Route("bpmnAudit")]
public class BpmnAuditController
{
    private readonly IBpmProcessAuditService _bpmProcessAuditService;

    public BpmnAuditController(IBpmProcessAuditService bpmProcessAuditService)
    {
        _bpmProcessAuditService = bpmProcessAuditService;
    }

    /// <summary>
    /// 按 processNumber 查询所有审计记录, 按 taskDefKey + createTime 升序.
    /// 返回字段: id / processNumber / formCode / fieldName / fieldLabel / oldValue / newValue /
    /// taskName / taskDefKey / createUser / createUserName / createTime.
    /// </summary>
    [HttpGet("list")]
    public Result<List<BpmProcessAudit>> List(string processNumber)
    {
        return ResultHelper.Success(_bpmProcessAuditService.GetProcessAudits(processNumber));
    }
}
