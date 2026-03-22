using antflowcore.bpmn.service;
using antflowcore.constant.enus;
using antflowcore.dto;
using antflowcore.entity;
using AntFlowCore.Entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.util.Extension;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.processoperation;

public class RemoveCurrentNodeProcessService: IProcessOperationAdaptor
{
    private readonly AFTaskService _afTaskService;
    private readonly TaskService _taskService;
    private readonly BpmFlowrunEntrustService _flowrunEntrustService;
    private readonly TaskMgmtService _taskMgmtService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly BpmvariableBizService _bpmvariableBizService;
    private readonly BpmVerifyInfoService _verifyInfoService;
    private readonly AFDeploymentService _deploymentService;

    public RemoveCurrentNodeProcessService(
        AFTaskService afTaskService,
        TaskService taskService,
        BpmFlowrunEntrustService flowrunEntrustService,
        TaskMgmtService taskMgmtService,
        BpmBusinessProcessService bpmBusinessProcessService,
        BpmvariableBizService bpmvariableBizService,
        BpmVerifyInfoService verifyInfoService,
        AFDeploymentService deploymentService)
    {
        _afTaskService = afTaskService;
        _taskService = taskService;
        _flowrunEntrustService = flowrunEntrustService;
        _taskMgmtService = taskMgmtService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _bpmvariableBizService = bpmvariableBizService;
        _verifyInfoService = verifyInfoService;
        _deploymentService = deploymentService;
    }

    public void DoProcessButton(BusinessDataVo vo)
    {
        string processNumber = vo.ProcessNumber;
        if (string.IsNullOrEmpty(processNumber))
        {
            throw new AFBizException("请指定流程编号");
        }

        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(processNumber);
        if (bpmBusinessProcess == null)
        {
            throw new AFBizException("未能根据流程编号找到流程信息!");
        }

        string processInstanceId = bpmBusinessProcess.ProcInstId;

        // 获取当前流程实例的所有任务
        List<BpmAfTask> bpmAfTasks = _afTaskService.baseRepo
            .Where(a => a.ProcInstId == processInstanceId)
            .ToList();

        if (bpmAfTasks.Count == 0)
        {
            throw new AFBizException("未找到当前流程任务!");
        }

        // 获取不同的 taskDefKey 列表
        List<string> taskDefKeys = bpmAfTasks
            .Select(a => a.TaskDefKey)
            .Distinct()
            .ToList();

        // 如果存在多个并行节点，无法移除当前节点
        if (taskDefKeys.Count > 1)
        {
            throw new AFBizException("当前流程存在多个并行节点,无法移除当前节点!");
        }

        string taskDefKey = taskDefKeys[0];

        // 获取当前节点ID
        NodeElementDto nodeElementDto = _bpmvariableBizService.GetNodeIdByElementId(processNumber, taskDefKey);
        string nodeId = nodeElementDto.NodeId;

        // 获取流程定义，判断当前节点的签收类型
        List<BpmnConfCommonElementVo> elements = _deploymentService.GetDeploymentByProcessNumber(processNumber);
        if (elements.IsEmpty())
        {
            throw new AFBizException($"未能根据流程编号找到流程定义!{processNumber}");
        }
        BpmnConfCommonElementVo currentElement = BpmnFlowUtil.GetCurrentTaskElement(elements, taskDefKey);
        if (currentElement == null)
        {
            throw new AFBizException($"未能根据节点elementId找到节点定义:{taskDefKey}");
        }

        int currentElementSignType = currentElement.SignType;
        bool isSignInOrder = (int)SignTypeEnum.SIGN_TYPE_SIGN_IN_ORDER == currentElementSignType;

        // 如果当前节点有多个任务（多人审批），删除除最后一个之外的其他任务
        if (bpmAfTasks.Count > 1)
        {
            // 删除除了最后一个任务外的所有任务
            for (int i = 0; i < bpmAfTasks.Count - 1; i++)
            {
                BpmAfTask task = bpmAfTasks[i];
                string executionId = task.ExecutionId;
                string taskId = task.Id;
                string assignee = task.Assignee;
                string assigneeName = task.AssigneeName;

                // 删除执行实例和任务
                _taskMgmtService.DeleteExecutionById(executionId);
                _taskMgmtService.DeletTask(taskId);

                // 记录减签操作日志
                _flowrunEntrustService.AddFlowrunEntrust(
                    "0",
                    "管理员删除当前节点",
                    assignee,
                    assigneeName,
                    taskDefKey,
                    0,
                    processInstanceId,
                    bpmBusinessProcess.ProcessinessKey,
                    nodeId,
                    3
                );
            }
        }

        // 完成最后一个任务，流程自动流转到下一个节点
        BpmAfTask lastTask = bpmAfTasks[bpmAfTasks.Count - 1];

        // 写入审批记录
        BpmVerifyInfo bpmVerifyInfo = new BpmVerifyInfo
        {
            VerifyDate = DateTime.Now,
            TaskName = lastTask.Name,
            TaskId = lastTask.Id,
            RunInfoId = processInstanceId,
            VerifyUserId = lastTask.Assignee,
            VerifyUserName = lastTask.AssigneeName,
            TaskDefKey = lastTask.TaskDefKey,
            VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
            VerifyDesc = "管理员删除当前节点",
            ProcessCode = processNumber,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };
        _verifyInfoService.AddVerifyInfo(bpmVerifyInfo);

        // 如果是顺序会签，需要修改流程定义为"跳过"，防止complete后继续生成其他顺序审批人任务
        if (isSignInOrder)
        {
            List<BaseInfoTranStructVo> assigneesToRemove = nodeElementDto.AssigneeInfoList.Where(a=>a.Id!=lastTask.Assignee).ToList();
            List<string> assigneeIds = assigneesToRemove.Select(a => a.Id).ToList();
            _bpmvariableBizService.InvalidNodeAssignees(assigneeIds, processNumber, nodeElementDto.IsSingle);
            List<BaseIdTranStruVo> baseIdTranStruVos = assigneesToRemove
                .Select(a=>(BaseIdTranStruVo)a)
                .Where(a=>a.Id!=lastTask.Assignee)
                .ToList();
            _deploymentService.UpdateNodeAssignee(processNumber, baseIdTranStruVos, nodeId, 2);
        }

        _taskService.Complete(lastTask);
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_REMOVE_CURRENT_NODE);
    }
}
