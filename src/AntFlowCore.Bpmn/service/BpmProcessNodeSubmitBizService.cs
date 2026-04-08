using System.Text.Json;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.exception;
using AntFlowCore.Common.util;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.interf;
using AntFlowCore.Core.util;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.service;

public class BpmProcessNodeSubmitBizService : IBpmProcessNodeSubmitBizService
{
    private readonly IBpmProcessNodeSubmitService _bpmProcessNodeSubmitService;
    private readonly IAFDeploymentService _afDeploymentService;
    private readonly ITaskService _taskService;
    private readonly IProcessNodeJumpService _processNodeJumpService;

    public BpmProcessNodeSubmitBizService(IBpmProcessNodeSubmitService bpmProcessNodeSubmitService,
        IAFDeploymentService afDeploymentService,ITaskService taskService,IProcessNodeJumpService processNodeJumpService)
    {
        _bpmProcessNodeSubmitService = bpmProcessNodeSubmitService;
        _afDeploymentService = afDeploymentService;
        _taskService = taskService;
        _processNodeJumpService = processNodeJumpService;
    }
    
     public void ProcessComplete(BpmAfTask task)
    {
        BpmProcessNodeSubmit processNodeSubmit = _bpmProcessNodeSubmitService.FindBpmProcessNodeSubmit(task.ProcInstId);
        if (processNodeSubmit != null)
        {
            _bpmProcessNodeSubmitService.AddProcessNode(new BpmProcessNodeSubmit
            {
                State = 0,
                NodeKey = task.TaskDefKey,
                ProcessInstanceId = task.ProcInstId,
                BackType = 0,
                CreateUser = SecurityUtils.GetLogInEmpName(),
                CreateTime = DateTime.Now,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            });
            bool nextElementParallelGateway=false;
            if (processNodeSubmit.BackType == 1||processNodeSubmit.BackType == 2 || processNodeSubmit.BackType == 4)
            {
                BpmAfDeployment bpmAfDeployment = _afDeploymentService.baseRepo.Where(a => a.Id == task.ProcDefId).First();
                if (bpmAfDeployment == null)
                {
                    throw new AFBizException($"can not find deployment by id: {task.ProcDefId}");
                }
                string content = bpmAfDeployment.Content;
                List<BpmnConfCommonElementVo> elements = JsonSerializer.Deserialize<List<BpmnConfCommonElementVo>>(content);
                var (assigneeNode, flowNode) = BpmnFlowUtil.GetNextNodeAndFlowNode(elements, task.TaskDefKey);
                if(assigneeNode!=null)
                {
                    nextElementParallelGateway = assigneeNode.ElementType == ElementTypeEnum.ELEMENT_TYPE_PARALLEL_GATEWAY.Code;
                }
            }
          
            if (processNodeSubmit.State==0
                ||(nextElementParallelGateway&&(processNodeSubmit.BackType == 1||processNodeSubmit.BackType==2||processNodeSubmit.BackType==4))) {
                _taskService.Complete(task);
            }
            else
            {
                Dictionary<string, object> varMap = new Dictionary<string, object>
                {
                    { StringConstants.TASK_ASSIGNEE_NAME, task.AssigneeName },
                    {StringConstants.VERIFY_COMMENT,"processNodeJump"},
                };
                switch (processNodeSubmit.BackType)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        _processNodeJumpService.CommitProcess(task,varMap,processNodeSubmit.NodeKey);
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
}