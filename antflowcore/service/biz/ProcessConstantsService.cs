using antflowcore.constant.enums;
using antflowcore.entity;
using antflowcore.exception;
using antflowcore.service.repository;
using antflowcore.util;
using antflowcore.vo;
using AntFlowCore.Entity;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.service.biz;

public class ProcessConstantsService
{
    private readonly AfTaskInstService _afTaskInstService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IBpmnEmployeeInfoProviderService _employeeInfoProviderService;
    private readonly TaskMgmtService _taskMgmtService;
    private readonly UserMessageService _userMessageService;
    private readonly BpmProcessForwardService _processForwardService;
    private readonly BpmnNodeLfFormdataFieldControlService _bpmnNodeLfFormdataFieldControlService;
    private readonly AFTaskService _afTaskService;
    private readonly ILogger<ProcessConstantsService> _logger;

    public ProcessConstantsService(AfTaskInstService afTaskInstService,
        BpmBusinessProcessService bpmBusinessProcessService,

        IBpmnEmployeeInfoProviderService employeeInfoProviderService,
        TaskMgmtService taskMgmtService,
        UserMessageService userMessageService,
        BpmProcessForwardService processForwardService,
        BpmnNodeLfFormdataFieldControlService bpmnNodeLfFormdataFieldControlService,
        AFTaskService afTaskService,
        ILogger<ProcessConstantsService> logger)
    {
        _afTaskInstService = afTaskInstService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _employeeInfoProviderService = employeeInfoProviderService;
        _taskMgmtService = taskMgmtService;
        _userMessageService = userMessageService;
        _processForwardService = processForwardService;
        _bpmnNodeLfFormdataFieldControlService = bpmnNodeLfFormdataFieldControlService;
        _afTaskService = afTaskService;
        _logger = logger;
    }

    public BpmAfTaskInst GetPrevTask(String taskDefKey, String procInstId)
    {
        if (string.IsNullOrEmpty(taskDefKey))
        {
            throw new ArgumentNullException(nameof(taskDefKey));
        }

        if (string.IsNullOrEmpty(procInstId))
        {
            throw new ArgumentNullException(nameof(procInstId));
        }

        List<BpmAfTaskInst> bpmAfTaskInsts = _afTaskInstService.baseRepo
            .Where(a => a.ProcInstId == procInstId && a.TaskDefKey == taskDefKey)
            .OrderByDescending(a => a.StartTime).ToList();
        BpmAfTaskInst? bpmAfTaskInst = bpmAfTaskInsts.Where(a => a.TaskDefKey != taskDefKey).FirstOrDefault();
        return bpmAfTaskInst;
    }

    public ProcessRecordInfoVo ProcessInfo(BpmBusinessProcess bpmBusinessProcess)
    {
        var processInfoVo = new ProcessRecordInfoVo();
        if (bpmBusinessProcess == null)
        {
            return processInfoVo;
        }

        // 检查权限
        if (!ShowProcessData(bpmBusinessProcess.BusinessNumber))
        {
            throw new AFBizException("00", "current user has no access right！");
        }

        // 设置任务状态
        processInfoVo.TaskState = ProcessStateEnumExtensions.GetDescByCode(bpmBusinessProcess.ProcessState);

        // 设置审核信息
        processInfoVo.VerifyInfoList = ServiceProviderUtils.GetService<BpmVerifyInfoService>().VerifyInfoList(bpmBusinessProcess.BusinessNumber, bpmBusinessProcess.ProcInstId);

        // 设置流程描述
        processInfoVo.ProcessTitle = bpmBusinessProcess.Description;

        // 获取员工信息
        var employee = _employeeInfoProviderService.QryLiteEmployeeInfoById(bpmBusinessProcess.CreateUser);
        processInfoVo.Employee = employee;
        processInfoVo.CreateTime = bpmBusinessProcess.CreateTime;

        // 设置发起人 ID
        processInfoVo.StartUserId = bpmBusinessProcess.CreateUser;

        // 设置流程编号
        processInfoVo.ProcessNumber = bpmBusinessProcess.BusinessNumber;

        string processInstanceId = bpmBusinessProcess.ProcInstId;

        // 修改转发数据
        _processForwardService.UpdateProcessForward(new BpmProcessForward
        {
            ProcessInstanceId = processInstanceId,
            ForwardUserId = SecurityUtils.GetLogInEmpIdStr()
        });

        // 修改通知
        _userMessageService.ReadNode(processInstanceId);

        // 查询当前用户的任务
        var tasks = _afTaskService
            .baseRepo
            .Where(a => a.ProcInstId == processInstanceId && a.Assignee == SecurityUtils.GetLogInEmpId())
            .ToList();
        string taskDefKey = "";

        if (tasks.Any())
        {
            var firstTask = tasks.First();
            taskDefKey = firstTask.TaskDefKey;
            processInfoVo.TaskId = firstTask.Id;
            processInfoVo.NodeId = taskDefKey;
        }
        else if (bpmBusinessProcess.IsLowCodeFlow == 1)
        {
            List<BpmAfTaskInst> historicTasks = _afTaskInstService
                .baseRepo
                .Where(a => a.ProcInstId == processInstanceId && a.Assignee == SecurityUtils.GetLogInEmpId())
                .OrderByDescending(a => a.EndTime)
                .ToList();

            if (historicTasks.Any())
            {
                taskDefKey = historicTasks.First().TaskDefKey;
            }
        }

        if (!string.IsNullOrEmpty(taskDefKey) && bpmBusinessProcess.IsLowCodeFlow == 1)
        {
            List<LFFieldControlVO> currentFieldControls = _bpmnNodeLfFormdataFieldControlService
                .GetFieldControlByProcessNumberAndElementId(bpmBusinessProcess.BusinessNumber, taskDefKey);

            processInfoVo.LfFieldControlVOs = currentFieldControls;
        }

        return processInfoVo;
    }

    public bool ShowProcessData(string processCode)
    {
        var bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(processCode);

        // 监控、查看、流程管理员、超级管理员、历史审批人和转发用户
        if (bpmBusinessProcess != null)
        {
            List<BpmAfTaskInst> taskInstanceList = _afTaskInstService
                .baseRepo
                .Where(a => a.ProcInstId == bpmBusinessProcess.ProcInstId)
                .ToList();

            var assigneeList = taskInstanceList
                .Where(task => task != null)
                .Select(task => task.Assignee)
                .ToList();

            if (assigneeList.Contains(SecurityUtils.GetLogInEmpIdStr()))
            {
                return true;
            }

            // TODO: 重新设计逻辑
            return true;
        }

        return true;
    }
}