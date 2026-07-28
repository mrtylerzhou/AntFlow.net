using System.Text.Json;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.interf;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.biz;

/// <summary>
/// 批量同意服务
/// </summary>
public class BatchApprovalService
{
    private readonly ITaskService _taskService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IProcessApprovalService _processApprovalService;
    private readonly ILogger<BatchApprovalService> _logger;

    public BatchApprovalService(
        ITaskService taskService,
        IBpmBusinessProcessService bpmBusinessProcessService,
        IProcessApprovalService processApprovalService,
        ILogger<BatchApprovalService> logger)
    {
        _taskService = taskService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _processApprovalService = processApprovalService;
        _logger = logger;
    }

    /// <summary>
    /// 批量同意
    /// </summary>
    public BatchAgreeResultVo BatchAgree(BatchAgreeVo vo)
    {
        var taskIds = vo.TaskIds;
        string comment = !string.IsNullOrWhiteSpace(vo.BatchApprovalComment)
            ? vo.BatchApprovalComment
            : "同意";
        string currentUserId = SecurityUtils.GetLogInEmpId();
        string currentUserName = SecurityUtils.GetLogInEmpName();

        var result = new BatchAgreeResultVo();

        foreach (var taskId in taskIds)
        {
            try
            {
                ExecuteSingleApproval(taskId, comment, currentUserId, currentUserName);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "批量同意失败, taskId={TaskId}", taskId);
                var failure = new BatchAgreeResultVo.FailureItem
                {
                    TaskId = taskId,
                    Reason = ex.Message
                };
                // 尝试补充流程编号和名称
                try
                {
                    var tasks = _taskService.CreateTaskQuery(t => t.Id == taskId);
                    if (tasks != null && tasks.Count > 0)
                    {
                        var bp = _bpmBusinessProcessService.GetBpmBusinessProcessByProcInstId(tasks[0].ProcInstId);
                        if (bp != null)
                        {
                            failure.ProcessNumber = bp.BusinessNumber;
                            failure.ProcessName = bp.Description;
                        }
                    }
                }
                catch { /* ignore */ }

                result.Failures.Add(failure);
            }
        }

        return result;
    }

    private void ExecuteSingleApproval(string taskId, string comment, string currentUserId, string currentUserName)
    {
        // 1. 查找任务
        var tasks = _taskService.CreateTaskQuery(t => t.Id == taskId);
        if (tasks == null || tasks.Count == 0)
        {
            throw new Exception("任务不存在或已完成");
        }

        var task = tasks[0];

        // 2. 安全校验: 验证当前用户是任务处理人
        if (task.Assignee != currentUserId)
        {
            throw new Exception("当前用户不是该任务的处理人");
        }

        // 3. 获取业务流程信息
        var bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcessByProcInstId(task.ProcInstId);
        if (bpmBusinessProcess == null)
        {
            throw new Exception("未找到关联的业务流程数据");
        }

        // 4. 构造审批参数
        var businessDataVo = new BusinessDataVo
        {
            OperationType = (int)ProcessOperationEnum.BUTTON_TYPE_AGREE,
            ApprovalComment = comment,
            ProcessNumber = bpmBusinessProcess.BusinessNumber,
            FormCode = bpmBusinessProcess.ProcessinessKey,
            ProcessKey = bpmBusinessProcess.ProcessinessKey,
            BusinessId = bpmBusinessProcess.BusinessId,
            TaskId = task.Id,
            TaskDefKey = task.TaskDefKey,
            IsLowCodeFlow = bpmBusinessProcess.IsLowCodeFlow,
            IsOutSideAccessProc = bpmBusinessProcess.IsOutSideProcess == 1,
            StartUserId = currentUserId,
            StartUserName = currentUserName
        };

        // 5. 执行审批操作（内部含事务）
        string json = JsonSerializer.Serialize(businessDataVo);
        _processApprovalService.ButtonsOperation(json, bpmBusinessProcess.ProcessinessKey);
    }
}
