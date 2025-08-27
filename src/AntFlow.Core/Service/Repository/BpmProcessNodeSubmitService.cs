using AntFlow.Core.Bpmn.Service;
using AntFlow.Core.Constant;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Business;
using AntFlow.Core.Service.Interface.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;
using System.Text.Json;

namespace AntFlow.Core.Service.Repository;

public class BpmProcessNodeSubmitService : AFBaseCurdRepositoryService<BpmProcessNodeSubmit>,
    IBpmProcessNodeSubmitService
{
    private readonly AFDeploymentService _afDeploymentService;
    private readonly ProcessNodeJumpService _processNodeJumpService;

    private readonly TaskService _taskService;

    public BpmProcessNodeSubmitService(
        TaskService taskService,
        ProcessNodeJumpService processNodeJumpService,
        AFDeploymentService afDeploymentService,
        IFreeSql freeSql) : base(freeSql)
    {
        _taskService = taskService;
        _processNodeJumpService = processNodeJumpService;
        _afDeploymentService = afDeploymentService;
    }

    public void ProcessComplete(BpmAfTask task)
    {
        BpmProcessNodeSubmit processNodeSubmit = FindBpmProcessNodeSubmit(task.ProcInstId);
        if (processNodeSubmit != null)
        {
            AddProcessNode(new BpmProcessNodeSubmit
            {
                State = 0,
                NodeKey = task.TaskDefKey,
                ProcessInstanceId = task.ProcInstId,
                BackType = 0,
                CreateUser = SecurityUtils.GetLogInEmpName(),
                CreateTime = DateTime.Now,
                TenantId = MultiTenantUtil.GetCurrentTenantId()
            });
            bool nextElementParallelGateway = false;
            if (processNodeSubmit.BackType == 1 || processNodeSubmit.BackType == 2 || processNodeSubmit.BackType == 4)
            {
                BpmAfDeployment bpmAfDeployment =
                    _afDeploymentService.baseRepo.Where(a => a.Id == task.ProcDefId).First();
                if (bpmAfDeployment == null)
                {
                    throw new AFBizException($"can not find deployment by id: {task.ProcDefId}");
                }

                string content = bpmAfDeployment.Content;
                List<BpmnConfCommonElementVo> elements =
                    JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(content);
                (BpmnConfCommonElementVo? assigneeNode, BpmnConfCommonElementVo? flowNode) =
                    BpmnFlowUtil.GetNextNodeAndFlowNode(elements, task.TaskDefKey);
                if (assigneeNode != null)
                {
                    nextElementParallelGateway =
                        assigneeNode.ElementType == ElementTypeEnum.ELEMENT_TYPE_PARALLEL_GATEWAY.Code;
                }
            }

            if (processNodeSubmit.State == 0
                || (nextElementParallelGateway && (processNodeSubmit.BackType == 1 || processNodeSubmit.BackType == 2 ||
                                                   processNodeSubmit.BackType == 4)))
            {
                _taskService.Complete(task);
            }
            else
            {
                Dictionary<string, object> varMap = new()
                {
                    { StringConstants.TASK_ASSIGNEE_NAME, task.AssigneeName },
                    { StringConstants.VERIFY_COMMENT, "processNodeJump" }
                };
                switch (processNodeSubmit.BackType)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        _processNodeJumpService.CommitProcess(task, varMap, processNodeSubmit.NodeKey);
                        break;
                    default:
                        _taskService.Complete(task);
                        break;
                }
            }
        }
        else
        {
            _taskService.Complete(task);
        }
    }

    public BpmProcessNodeSubmit FindBpmProcessNodeSubmit(string processInstanceId)
    {
        BpmProcessNodeSubmit bpmProcessNodeSubmit = baseRepo
            .Where(a => a.ProcessInstanceId.Equals(processInstanceId))
            .OrderByDescending(a => a.CreateTime)
            .First();
        return bpmProcessNodeSubmit;
    }

    public bool AddProcessNode(BpmProcessNodeSubmit processNodeSubmit)
    {
        baseRepo.Delete(a => a.ProcessInstanceId == processNodeSubmit.ProcessInstanceId);
        baseRepo.Insert(processNodeSubmit);
        return true;
    }
}