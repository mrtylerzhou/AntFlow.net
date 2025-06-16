using System.Text.Json;
using antflowcore.bpmn;
using antflowcore.bpmn.service;
using antflowcore.constant.enus;
using AntFlowCore.Constants;
using antflowcore.entity;
using AntFlowCore.Entity;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.service.repository;

public class BpmProcessNodeSubmitService: AFBaseCurdRepositoryService<BpmProcessNodeSubmit>
{
   
    private readonly TaskService _taskService;
    private readonly ProcessNodeJumpService _processNodeJumpService;
    private readonly AFDeploymentService _afDeploymentService;

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
        BpmProcessNodeSubmit processNodeSubmit = this.FindBpmProcessNodeSubmit(task.ProcInstId);
        if (processNodeSubmit != null)
        {
            this.AddProcessNode(new BpmProcessNodeSubmit
            {
                State = 0,
                NodeKey = task.TaskDefKey,
                ProcessInstanceId = task.ProcInstId,
                BackType = 0,
                CreateUser = SecurityUtils.GetLogInEmpName()
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