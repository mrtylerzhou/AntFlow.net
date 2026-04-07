using System.Text.Json;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Bpmn;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.exception;
using AntFlowCore.Common.util;
using AntFlowCore.Constants;
using AntFlowCore.Core.entity;
using AntFlowCore.Entity;
using AntFlowCore.Extensions.service;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;

namespace AntFlowCore.Persist.repository;

public class BpmProcessNodeSubmitService: AFBaseCurdRepositoryService<BpmProcessNodeSubmit>,IBpmProcessNodeSubmitService
{
   
    private readonly ITaskService _taskService;
    private readonly IProcessNodeJumpService _processNodeJumpService;
    private readonly AFDeploymentService _afDeploymentService;

    public BpmProcessNodeSubmitService(
        ITaskService taskService,
        IProcessNodeJumpService processNodeJumpService,
        AFDeploymentService afDeploymentService,
        IFreeSql freeSql) : base(freeSql)
    {
        _taskService = taskService;
        _processNodeJumpService = processNodeJumpService;
        _afDeploymentService = afDeploymentService;
    }

   

    public BpmProcessNodeSubmit FindBpmProcessNodeSubmit(String processInstanceId)
    {
        BpmProcessNodeSubmit bpmProcessNodeSubmit = this
            .baseRepo
            .Where(a=>a.ProcessInstanceId.Equals(processInstanceId))
            .OrderByDescending(a=>a.CreateTime)
            .First();
        return bpmProcessNodeSubmit;
    }
    public bool AddProcessNode(BpmProcessNodeSubmit processNodeSubmit) {
        this.baseRepo.Delete(a => a.ProcessInstanceId == processNodeSubmit.ProcessInstanceId);
        this.baseRepo.Insert(processNodeSubmit);
        return true;
    }
}