using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.processoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.service;
using AntFlowCore.Persist.api.interf.repository;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

/// <summary>
/// 审批人推进操作:先同意当前任务,再跳转到未来节点.
/// 独立于管理员推进(FastForwardProcessService, code=33).
/// 对应 Java ForwardToNodeImpl (code=42).
///
/// 执行逻辑:
/// 1. complete当前任务 + 记录"同意"审批日志
/// 2. 查询新的当前taskDefKey(complete后引擎推进一个节点)
/// 3. 判断是否跨并行网关:
///    - 不跨: TurnTransition(新当前task, 目标taskDefKey)
///    - 跨: 递归complete中间任务(记录"推进跳过")
/// </summary>
public class ForwardToNodeService : IProcessOperationAdaptor
{
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IAFTaskService _afTaskInstService;
    private readonly IBpmVerifyInfoService _bpmVerifyInfoService;
    private readonly IBpmVariableService _bpmVariableService;
    private readonly IBpmFlowrunEntrustService _bpmFlowrunEntrustService;
    private readonly IProcessNodeJumpService _processNodeJump;
    private readonly TaskService _taskService;

    public ForwardToNodeService(
        IBpmBusinessProcessService bpmBusinessProcessService,
        IAFTaskService afTaskInstService,
        IBpmVerifyInfoService bpmVerifyInfoService,
        IBpmVariableService bpmVariableService,
        IBpmFlowrunEntrustService bpmFlowrunEntrustService,
        IProcessNodeJumpService processNodeJump,
        TaskService taskService)
    {
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _afTaskInstService = afTaskInstService;
        _bpmVerifyInfoService = bpmVerifyInfoService;
        _bpmVariableService = bpmVariableService;
        _bpmFlowrunEntrustService = bpmFlowrunEntrustService;
        _processNodeJump = processNodeJump;
        _taskService = taskService;
    }

    public void DoProcessButton(BusinessDataVo vo)
    {
        string processNumber = vo.ProcessNumber;
        if (string.IsNullOrEmpty(processNumber))
        {
            throw new AFBizException("流程编号不能为空");
        }

        // 目标节点: 前端传入nodeId(主键id), 需转换为elementId(taskDefKey)
        string targetNodeId = vo.ForwardToNodeId;
        if (string.IsNullOrEmpty(targetNodeId))
        {
            throw new AFBizException("推进目标节点不能为空");
        }

        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(processNumber);
        if (bpmBusinessProcess == null)
        {
            throw new AFBizException($"未找到流程信息,流程编号:{processNumber}");
        }
        string procInstId = bpmBusinessProcess.ProcInstId;

        // 获取当前任务
        List<BpmAfTask> currentTasks = _afTaskInstService._repository
            .Find(a => a.ProcInstId == procInstId);
        if (currentTasks.IsEmpty())
        {
            throw new AFBizException("未获取到当前流程任务!");
        }

        // 将目标nodeId转换为elementId(taskDefKey)
        List<string> targetElementIds = _bpmVariableService.GetElementIdsdByNodeId(processNumber, targetNodeId);
        if (targetElementIds.IsEmpty())
        {
            throw new AFBizException($"未能根据nodeId获取目标节点taskDefKey:{targetNodeId}");
        }
        string targetTaskDefKey = targetElementIds[0];

        // Step 1: complete当前任务(同意) + 记录审批日志
        string loginEmpId = SecurityUtils.GetLogInEmpId();
        string loginEmpName = SecurityUtils.GetLogInEmpName();
        BpmAfTask currentTask = currentTasks.FirstOrDefault(t => t.Assignee == loginEmpId) ?? currentTasks[0];

        _taskService.Complete(currentTask);

        // 记录"同意"审批日志(含推进信息)
        string comment = vo.ApprovalComment;
        BpmVerifyInfo verifyInfo = new BpmVerifyInfo
        {
            VerifyDate = DateTime.Now,
            TaskName = currentTask.Name,
            TaskId = currentTask.Id,
            RunInfoId = procInstId,
            VerifyUserId = loginEmpId,
            VerifyUserName = loginEmpName,
            TaskDefKey = currentTask.TaskDefKey,
            VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
            VerifyDesc = "同意(推进至目标节点)" + (string.IsNullOrEmpty(comment) ? "" : ",意见:" + comment),
            ProcessCode = processNumber,
        };
        _bpmVerifyInfoService.AddVerifyInfo(verifyInfo);

        // Step 2: 查询complete后的新当前任务
        List<BpmAfTask> newTasks = _afTaskInstService._repository
            .Find(a => a.ProcInstId == procInstId);
        if (newTasks.IsEmpty())
        {
            // 流程已结束(complete后没有新任务),无需推进
            return;
        }

        // 检查新当前任务是否已经就是目标节点
        var newTaskDefKeys = newTasks.Select(t => t.TaskDefKey).Distinct().ToList();
        if (newTaskDefKeys.Count == 1 && newTaskDefKeys[0] == targetTaskDefKey)
        {
            // 目标就是下一个节点,无需额外跳转
            return;
        }

        // Step 3: 判断是否跨并行网关
        if (newTaskDefKeys.Count == 1)
        {
            // 顺序流: 使用TurnTransition直接跳转
            BpmAfTask newCurrentTask = newTasks[0];
            var variables = new Dictionary<string, object>
            {
                { StringConstants.VERIFY_COMMENT, "推进跳转" }
            };
            _processNodeJump.TurnTransition(newCurrentTask, targetTaskDefKey, null, variables);
        }
        else
        {
            // 并行流(多个taskDefKey): 使用递归complete方式推进
            RecursiveCompleteToTarget(newTasks, procInstId, targetTaskDefKey,
                processNumber, comment, bpmBusinessProcess.ProcessinessKey);
        }
    }

    /// <summary>
    /// 自动推进节点专用:complete 当前任务后推进到指定目标 elementId.
    /// 与人工推进 DoProcessButton 的差异:
    /// - 入参为 delegateTask(由 NextNodeLabelsProcessor.PostProcess 直接提供),无需再查当前任务列表
    /// - 目标节点已转换为 elementId(taskDefKey),由调用方负责 UUID→主键→elementId 两步转换
    /// - verifyUserId/verifyUserName 由调用方传入(自动推进用虚拟人 -3)
    ///
    /// .NET 版直接同步执行 complete + TurnTransition(不需要 Java 版的 afterCommit 延迟,
    /// 因为 .NET 的 TaskService.Complete 是直接 DB 操作,无 Activiti CommandContext 缓冲)。
    /// </summary>
    public void AdvanceToTargetNode(BpmAfTask delegateTask, string procInstId,
        string currentTaskDefKey, string targetElementId, string targetNodeName,
        string verifyUserId, string verifyUserName, string processNumber)
    {
        // 1. 设置审批人为虚拟人
        delegateTask.Assignee = verifyUserId;
        delegateTask.AssigneeName = verifyUserName;

        // 2. complete 当前任务
        _taskService.Complete(delegateTask);

        // 3. 记录审批日志
        BpmVerifyInfo verifyInfo = new BpmVerifyInfo
        {
            VerifyDate = DateTime.Now,
            TaskName = delegateTask.Name,
            TaskId = delegateTask.Id,
            RunInfoId = procInstId,
            VerifyUserId = verifyUserId,
            VerifyUserName = verifyUserName,
            TaskDefKey = delegateTask.TaskDefKey,
            VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
            VerifyDesc = $"自动推进:条件满足,推进至节点[{targetNodeName}]",
            ProcessCode = processNumber,
        };
        _bpmVerifyInfoService.AddVerifyInfo(verifyInfo);

        // 4. 查 complete 后的新任务
        List<BpmAfTask> newTasks = _afTaskInstService._repository
            .Find(a => a.ProcInstId == procInstId);

        // 5. 如果空(流程已结束)return
        if (newTasks.IsEmpty())
        {
            return;
        }

        // 6. 取 newTaskDefKeys
        var newTaskDefKeys = newTasks.Select(t => t.TaskDefKey).Distinct().ToList();

        // 7. 如果 newTaskDefKeys.Count==1 且 == targetElementId,return(目标已是下一节点)
        if (newTaskDefKeys.Count == 1 && newTaskDefKeys[0] == targetElementId)
        {
            return;
        }

        // 8. 否则推进
        if (newTaskDefKeys.Count == 1)
        {
            // 顺序流: 使用 TurnTransition 直接跳转
            BpmAfTask newCurrentTask = newTasks[0];
            var variables = new Dictionary<string, object>
            {
                { StringConstants.VERIFY_COMMENT, "推进跳转" }
            };
            _processNodeJump.TurnTransition(newCurrentTask, targetElementId, null, variables);
        }
        else
        {
            // 并行流(多个taskDefKey): 使用递归 complete 方式推进
            RecursiveCompleteToTarget(newTasks, procInstId, targetElementId,
                processNumber, null, delegateTask.ProcDefId);
        }
    }

    /// <summary>
    /// 跨并行网关: 递归complete中间任务直到目标节点
    /// </summary>
    private void RecursiveCompleteToTarget(List<BpmAfTask> taskList, string processInstanceId,
        string forwardToNodeElementId, string processNumber, string verifyComment, string processKey)
    {
        if (taskList.IsEmpty())
        {
            return;
        }

        string loginEmpId = SecurityUtils.GetLogInEmpId();
        string loginEmpName = SecurityUtils.GetLogInEmpName();

        foreach (BpmAfTask task in taskList)
        {
            // 如果当前任务已经是目标节点或目标之后,停止
            if (task.TaskDefKey == forwardToNodeElementId)
            {
                return;
            }
            if (ProcessNodeEnum.Compare(task.TaskDefKey, forwardToNodeElementId) > 0)
            {
                return;
            }

            _taskService.Complete(task);

            // 记录"推进跳过"审批日志
            BpmVerifyInfo bpmVerifyInfo = new BpmVerifyInfo
            {
                VerifyDate = DateTime.Now,
                TaskName = task.Name,
                TaskId = task.Id,
                RunInfoId = processInstanceId,
                VerifyUserId = loginEmpId,
                VerifyUserName = loginEmpName,
                TaskDefKey = task.TaskDefKey,
                VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
                VerifyDesc = "推进跳过,原因:" + (verifyComment ?? ""),
                ProcessCode = processNumber,
            };
            _bpmVerifyInfoService.AddVerifyInfo(bpmVerifyInfo);

            // 委托记录
            List<string> nodeIdsByeElementId = _bpmVariableService.GetNodeIdsByeElementId(processNumber, task.TaskDefKey);
            string nodeId = nodeIdsByeElementId.IsEmpty() ? "" : nodeIdsByeElementId[0];
            _bpmFlowrunEntrustService.AddFlowrunEntrust(loginEmpId, loginEmpName,
                task.Assignee, task.AssigneeName, task.TaskDefKey, 0,
                processInstanceId, processKey, nodeId, 1);
        }

        // 递归: complete后查询新任务继续推进
        List<BpmAfTask> tasks = _afTaskInstService._repository
            .Find(a => a.ProcInstId == processInstanceId);
        RecursiveCompleteToTarget(tasks, processInstanceId, forwardToNodeElementId,
            processNumber, verifyComment, processKey);
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_FORWARD_TO_NODE);
    }
}
