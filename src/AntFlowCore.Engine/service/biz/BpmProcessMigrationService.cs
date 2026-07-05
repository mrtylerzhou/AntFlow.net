using System.Text.Json;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.interf;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.biz;

/// <summary>
/// Implementation of <see cref="IBpmProcessMigrationService"/>.
/// When dynamic conditions change during resubmit, this service:
/// 1. Re-submits the process (starts a new process instance with updated form data)
/// 2. Iterates through the new process's tasks, completing each one up to the
///    current task definition key (replaying the approval history)
/// </summary>
public class BpmProcessMigrationService : IBpmProcessMigrationService
{
    private readonly IProcessApprovalService _processApprovalService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly ITaskService _taskService;
    private readonly IBpmVerifyInfoService _bpmVerifyInfoService;
    private readonly ILogger<BpmProcessMigrationService> _logger;

    public BpmProcessMigrationService(
        IProcessApprovalService processApprovalService,
        IBpmBusinessProcessService bpmBusinessProcessService,
        ITaskService taskService,
        IBpmVerifyInfoService bpmVerifyInfoService,
        ILogger<BpmProcessMigrationService> logger)
    {
        _processApprovalService = processApprovalService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _taskService = taskService;
        _bpmVerifyInfoService = bpmVerifyInfoService;
        _logger = logger;
    }

    public void MigrateAndJumpToCurrent(BpmAfTask currentTask, BpmBusinessProcess bpmBusinessProcess,
        BusinessDataVo vo, Action<BusinessDataVo, BpmAfTask, BpmBusinessProcess> taskCompletionAction)
    {
        string currentTaskDefKey = currentTask.TaskDefKey;
        string currentComment = vo.ApprovalComment;

        // Build a submit VO for re-submitting the process
        var submitVo = JsonSerializer.Deserialize<BusinessDataVo>(JsonSerializer.Serialize(vo));
        submitVo.IsLowCodeFlow = bpmBusinessProcess.IsLowCodeFlow;
        submitVo.StartUserId = bpmBusinessProcess.CreateUser;
        submitVo.BpmnCode = bpmBusinessProcess.Version;
        submitVo.OperationType = (int)ProcessOperationEnum.BUTTON_TYPE_SUBMIT;
        // Mark as migration so condition service knows to re-evaluate (not record)
        // and condition filter doesn't prune dynamic condition branches

        // Re-submit the process (starts a new process instance)
        string submitJson = JsonSerializer.Serialize(submitVo);
        _processApprovalService.ButtonsOperation(submitJson, submitVo.FormCode);

        // Get the updated business process (new procInstId after re-submit)
        bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);
        if (bpmBusinessProcess == null)
        {
            throw new AFBizException("迁移后未能找到业务流程信息!");
        }

        // Get all tasks for the new process instance
        List<BpmAfTask> tasks = _taskService.CreateTaskQuery(t =>
            t.ProcInstId == bpmBusinessProcess.ProcInstId);

        if (tasks == null || tasks.Count == 0)
        {
            throw new AFBizException("迁移后未找到流程任务!");
        }

        // Replay: complete tasks up to and including the current task definition key
        // Get verify info history to know which tasks were already approved
        var verifyInfoMap = _bpmVerifyInfoService._repository
            .Find(a => a.ProcessCode == vo.ProcessNumber)
            .ToList()
            .ToDictionary(a => a.TaskDefKey + a.VerifyUserId, a => a);

        bool currentExecuted = false;
        int index = 0;
        foreach (var task in tasks)
        {
            if (currentExecuted)
            {
                break;
            }

            // 修复(dc0bf5ee8): 最后一个任务的 operationType 设为 AGREE,
            // 否则最后一个任务的审批记录类型不正确
            if (index == tasks.Count - 1)
            {
                vo.OperationType = (int)ProcessOperationEnum.BUTTON_TYPE_AGREE;
            }

            // Check if this task was previously approved
            string verifyKey = task.TaskDefKey + task.Assignee;
            BpmVerifyInfo? prevVerifyInfo = null;
            verifyInfoMap.TryGetValue(verifyKey, out prevVerifyInfo);

            vo.StartUserId = task.Assignee;
            if (prevVerifyInfo != null)
            {
                // Previously approved — auto-complete with "already processed" comment
                if (!string.IsNullOrEmpty(task.AssigneeName))
                {
                    vo.StartUserName = task.AssigneeName;
                }
                else
                {
                    vo.StartUserName = prevVerifyInfo.VerifyUserName;
                }
                vo.ApprovalComment = StringConstants.CURRENT_USER_ALREADY_PROCESSED;
            }
            else
            {
                // Not previously approved — this might be the current task or a new task
                if (!string.IsNullOrEmpty(task.AssigneeName))
                {
                    vo.StartUserName = task.AssigneeName;
                }
                if (currentTaskDefKey == task.TaskDefKey)
                {
                    vo.ApprovalComment = currentComment;
                }
            }

            taskCompletionAction(vo, task, bpmBusinessProcess);

            if (currentTaskDefKey == task.TaskDefKey)
            {
                currentExecuted = true;
            }
            index++;
        }
    }
}
