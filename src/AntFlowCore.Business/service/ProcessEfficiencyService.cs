using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Business.service;

/// <summary>
/// 流程效能统计 Service
/// </summary>
public class ProcessEfficiencyService
{
    private readonly IBpmProcessEfficiencyRepository _efficiencyRepository;
    private readonly IBpmAfTaskInstRepository _taskInstRepository;
    private readonly IBpmBusinessProcessRepository _businessProcessRepository;
    private readonly IBpmVariableMultiplayerRepository _multiplayerRepository;
    private readonly IBpmnNodeRepository _nodeRepository;
    private readonly IUserService _userService;
    private readonly ILogger<ProcessEfficiencyService> _logger;

    public ProcessEfficiencyService(
        IBpmProcessEfficiencyRepository efficiencyRepository,
        IBpmAfTaskInstRepository taskInstRepository,
        IBpmBusinessProcessRepository businessProcessRepository,
        IBpmVariableMultiplayerRepository multiplayerRepository,
        IBpmnNodeRepository nodeRepository,
        IUserService userService,
        ILogger<ProcessEfficiencyService> logger)
    {
        _efficiencyRepository = efficiencyRepository;
        _taskInstRepository = taskInstRepository;
        _businessProcessRepository = businessProcessRepository;
        _multiplayerRepository = multiplayerRepository;
        _nodeRepository = nodeRepository;
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// 执行效能统计计算
    /// </summary>
    public void CalculateEfficiency(List<string> formCodes)
    {
        DateTime todayStart = DateTime.Today;
        DateTime yesterdayStart = todayStart.AddDays(-1);

        // 查询昨天更新的流程
        var processes = _businessProcessRepository.Find(a =>
            a.UpdateTime >= yesterdayStart && a.UpdateTime < todayStart);

        if (formCodes != null && formCodes.Count > 0)
        {
            processes = processes.Where(a => formCodes.Contains(a.ProcessinessKey)).ToList();
        }

        if (processes.Count == 0)
        {
            _logger.LogInformation("效能统计:未查询到昨天更新的流程数据");
            return;
        }

        _logger.LogInformation("效能统计:查询到{Count}条待处理流程", processes.Count);

        foreach (var process in processes)
        {
            try
            {
                ProcessSingleProcess(process);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "效能统计:处理流程[{ProcessNumber}]异常", process.BusinessNumber);
            }
        }
    }

    private void ProcessSingleProcess(BpmBusinessProcess process)
    {
        string processNumber = process.BusinessNumber;
        int processState = process.ProcessState;
        bool isHandling = processState == (int)ProcessStateEnum.HANDLING_STATE;

        // 检查是否已有统计记录
        int existCount = _efficiencyRepository.Count(a => a.ProcessNumber == processNumber);

        if (existCount > 0)
        {
            if (!isHandling)
            {
                // 已终结且已统计 -> 跳过
                return;
            }
            // 审批中且已统计 -> 删除旧记录重新计算
            _efficiencyRepository.DeleteByProcessNumber(processNumber);
        }

        // 查询该流程的所有历史任务
        string procInstId = process.ProcInstId;
        if (string.IsNullOrEmpty(procInstId))
        {
            _logger.LogWarning("效能统计:流程[{ProcessNumber}]无流程实例ID,跳过", processNumber);
            return;
        }

        List<BpmAfTaskInst> tasks = _taskInstRepository.Find(a => a.ProcInstId == procInstId);
        if (tasks.Count == 0)
        {
            _logger.LogWarning("效能统计:流程[{ProcessNumber}]无历史任务记录,跳过", processNumber);
            return;
        }

        string formCode = process.ProcessinessKey;
        DateTime? processCreateTime = process.CreateTime;
        string tenantId = MultiTenantUtil.GetCurrentTenantId();
        DateTime now = DateTime.Now;
        var records = new List<BpmProcessEfficiency>();

        // === 任务级别 ===
        foreach (var task in tasks)
        {
            DateTime startTime = task.StartTime;
            DateTime? endTime = task.EndTime;
            long duration;
            if (endTime.HasValue)
            {
                duration = (long)(endTime.Value - startTime).TotalMilliseconds;
            }
            else
            {
                duration = (long)(now - startTime).TotalMilliseconds;
            }

            string nodeName = ResolveNodeName(processNumber, task.TaskDefKey, task.Name);
            string assigneeName = ResolveAssigneeName(task);

            records.Add(new BpmProcessEfficiency
            {
                FormCode = formCode,
                ProcessNumber = processNumber,
                ProcInstId = procInstId,
                ExecutionId = task.ExecutionId,
                TaskDefKey = task.TaskDefKey,
                NodeName = nodeName,
                Assignee = task.Assignee,
                AssigneeName = assigneeName,
                StaticType = BpmProcessEfficiency.TYPE_TASK,
                StartTime = startTime,
                EndTime = endTime,
                Duration = duration,
                ProcessState = processState,
                ProcessCreateTime = processCreateTime,
                TenantId = tenantId,
                IsDel = 0,
                CreateTime = now,
                UpdateTime = now
            });
        }

        // === 节点级别 ===
        var nodeGroup = tasks
            .Where(t => !string.IsNullOrEmpty(t.TaskDefKey))
            .GroupBy(t => t.TaskDefKey);

        foreach (var group in nodeGroup)
        {
            string taskDefKey = group.Key;
            var nodeTasks = group.ToList();

            DateTime? minStart = nodeTasks
                .Where(t => t.StartTime != default)
                .Min(t => (DateTime?)t.StartTime);
            bool hasUnfinished = nodeTasks.Any(t => t.EndTime == null);
            DateTime? maxEnd = hasUnfinished ? null : nodeTasks
                .Where(t => t.EndTime.HasValue)
                .Max(t => t.EndTime);

            long duration;
            if (minStart == null)
            {
                duration = 0;
            }
            else if (maxEnd.HasValue)
            {
                duration = (long)(maxEnd.Value - minStart.Value).TotalMilliseconds;
            }
            else
            {
                duration = (long)(now - minStart.Value).TotalMilliseconds;
            }

            string nodeName = ResolveNodeName(processNumber, taskDefKey, nodeTasks[0].Name);
            string assignees = string.Join(",",
                nodeTasks.Where(t => !string.IsNullOrEmpty(t.Assignee))
                    .Select(t => t.Assignee).Distinct());
            string assigneeNames = string.Join(",",
                nodeTasks.Select(t => ResolveAssigneeName(t))
                    .Where(n => !string.IsNullOrEmpty(n)).Distinct());

            records.Add(new BpmProcessEfficiency
            {
                FormCode = formCode,
                ProcessNumber = processNumber,
                ProcInstId = procInstId,
                ExecutionId = nodeTasks[0].ExecutionId,
                TaskDefKey = taskDefKey,
                NodeName = nodeName,
                Assignee = assignees,
                AssigneeName = assigneeNames,
                StaticType = BpmProcessEfficiency.TYPE_NODE,
                StartTime = minStart,
                EndTime = maxEnd,
                Duration = duration,
                ProcessState = processState,
                ProcessCreateTime = processCreateTime,
                TenantId = tenantId,
                IsDel = 0,
                CreateTime = now,
                UpdateTime = now
            });
        }

        // === 流程级别 ===
        bool processFinished = !isHandling;
        DateTime? processEndTime = null;
        if (processFinished)
        {
            processEndTime = tasks
                .Where(t => t.EndTime.HasValue)
                .Max(t => t.EndTime);
        }

        long processDuration;
        if (processCreateTime == null)
        {
            processDuration = 0;
        }
        else if (processEndTime.HasValue)
        {
            processDuration = (long)(processEndTime.Value - processCreateTime.Value).TotalMilliseconds;
        }
        else
        {
            processDuration = (long)(now - processCreateTime.Value).TotalMilliseconds;
        }

        records.Add(new BpmProcessEfficiency
        {
            FormCode = formCode,
            ProcessNumber = processNumber,
            ProcInstId = procInstId,
            ExecutionId = null,
            TaskDefKey = null,
            NodeName = null,
            Assignee = null,
            AssigneeName = null,
            StaticType = BpmProcessEfficiency.TYPE_PROCESS,
            StartTime = processCreateTime,
            EndTime = processEndTime,
            Duration = processDuration,
            ProcessState = processState,
            ProcessCreateTime = processCreateTime,
            TenantId = tenantId,
            IsDel = 0,
            CreateTime = now,
            UpdateTime = now
        });

        // 批量写入
        _efficiencyRepository.AddRange(records);
        _logger.LogInformation("效能统计:流程[{ProcessNumber}]统计完成,写入{Count}条记录", processNumber, records.Count);
    }

    /// <summary>
    /// 解析节点名称:优先从node表取,取不到降级用任务Name
    /// </summary>
    private string ResolveNodeName(string processNumber, string taskDefKey, string taskName)
    {
        if (string.IsNullOrEmpty(taskDefKey))
        {
            return taskName;
        }
        try
        {
            var multiplayers = _multiplayerRepository
                .QueryMultiplayersByProcessNumAndElementId(processNumber, taskDefKey);
            if (multiplayers.Count > 0 && !string.IsNullOrEmpty(multiplayers[0].NodeId))
            {
                string nodeId = multiplayers[0].NodeId;
                if (long.TryParse(nodeId, out long nodeIdLong))
                {
                    var node = _nodeRepository.GetById(nodeIdLong);
                    if (node != null && !string.IsNullOrEmpty(node.NodeName))
                    {
                        return node.NodeName;
                    }
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogDebug(e, "效能统计:获取节点名称失败,processNumber={ProcessNumber},taskDefKey={TaskDefKey}",
                processNumber, taskDefKey);
        }
        return taskName;
    }

    /// <summary>
    /// 解析审批人姓名:优先取AssigneeName,为空则调UserService
    /// </summary>
    private string ResolveAssigneeName(BpmAfTaskInst task)
    {
        if (!string.IsNullOrEmpty(task.AssigneeName))
        {
            return task.AssigneeName;
        }
        if (!string.IsNullOrEmpty(task.Assignee))
        {
            try
            {
                var users = _userService.QueryUserByIds(new List<string> { task.Assignee });
                if (users.Count > 0 && users[0] != null)
                {
                    return users[0].Name;
                }
            }
            catch (Exception e)
            {
                _logger.LogDebug(e, "效能统计:获取审批人姓名失败,assignee={Assignee}", task.Assignee);
            }
        }
        return null;
    }

    // ==================== 查询接口 ====================

    /// <summary>
    /// 分页查询流程级效能数据
    /// </summary>
    public ResultAndPage<BpmProcessEfficiency> PageProcessLevel(ProcessEfficiencyVo vo)
    {
        PageDto pageDto = vo.PageDto ?? new PageDto();
        string tenantId = MultiTenantUtil.GetCurrentTenantId();

        List<string> procInstIds = null;
        if (!string.IsNullOrEmpty(vo.Assignee))
        {
            // 流程级不存审批人,需通过子级匹配
            var matched = _efficiencyRepository.Find(a =>
                a.StaticType == BpmProcessEfficiency.TYPE_TASK &&
                (a.AssigneeName.Contains(vo.Assignee) || a.Assignee.Contains(vo.Assignee)));
            if (matched.Count == 0)
            {
                return new ResultAndPage<BpmProcessEfficiency>(
                    new List<BpmProcessEfficiency>(),
                    PageDto.BuildCountedPage(pageDto, 0));
            }
            procInstIds = matched.Select(a => a.ProcInstId).Distinct().ToList();
        }

        var (data, total) = _efficiencyRepository.PageProcessLevel(
            tenantId, vo.FormCode, vo.ProcessNumber,
            vo.ProcessState, vo.StartTimeBegin, vo.StartTimeEnd,
            procInstIds, pageDto.Page, pageDto.PageSize);

        return new ResultAndPage<BpmProcessEfficiency>(data, PageDto.BuildCountedPage(pageDto, total));
    }

    /// <summary>
    /// 查询节点级效能数据
    /// </summary>
    public List<BpmProcessEfficiency> ListNodeLevel(string procInstId)
    {
        return _efficiencyRepository.Find(a =>
            a.ProcInstId == procInstId &&
            a.StaticType == BpmProcessEfficiency.TYPE_NODE)
            .OrderBy(a => a.StartTime).ToList();
    }

    /// <summary>
    /// 查询任务级效能数据
    /// </summary>
    public List<BpmProcessEfficiency> ListTaskLevel(string procInstId, string taskDefKey)
    {
        return _efficiencyRepository.Find(a =>
            a.ProcInstId == procInstId &&
            a.TaskDefKey == taskDefKey &&
            a.StaticType == BpmProcessEfficiency.TYPE_TASK)
            .OrderBy(a => a.StartTime).ToList();
    }
}
