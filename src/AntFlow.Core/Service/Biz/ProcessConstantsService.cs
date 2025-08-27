using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Business;

public class ProcessConstantsService
{
    private readonly AfTaskInstService _afTaskInstService;
    private readonly AFTaskService _afTaskService;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly BpmnNodeLfFormdataFieldControlService _bpmnNodeLfFormdataFieldControlService;
    private readonly IBpmnEmployeeInfoProviderService _employeeInfoProviderService;
    private readonly ILogger<ProcessConstantsService> _logger;
    private readonly BpmProcessForwardService _processForwardService;
    private readonly TaskMgmtService _taskMgmtService;
    private readonly UserMessageService _userMessageService;

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

    public BpmAfTaskInst GetPrevTask(string taskDefKey, string procInstId)
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
            .Where(a => a.ProcInstId == procInstId && a.TaskDefKey == taskDefKey && a.EndTime != null)
            .OrderByDescending(a => a.StartTime).ToList();
        BpmAfTaskInst? bpmAfTaskInst = bpmAfTaskInsts.Where(a => a.TaskDefKey != taskDefKey).FirstOrDefault();
        return bpmAfTaskInst;
    }

    public ProcessRecordInfoVo ProcessInfo(BpmBusinessProcess bpmBusinessProcess)
    {
        ProcessRecordInfoVo? processInfoVo = new();
        if (bpmBusinessProcess == null)
        {
            return processInfoVo;
        }

        // ??????
        if (!ShowProcessData(bpmBusinessProcess.BusinessNumber))
        {
            throw new AFBizException("00", "current user has no access right??");
        }

        // ??????????
        processInfoVo.TaskState = ProcessStateEnumExtensions.GetDescByCode(bpmBusinessProcess.ProcessState);

        // ??????????
        processInfoVo.VerifyInfoList = ServiceProviderUtils.GetService<BpmVerifyInfoService>()
            .VerifyInfoList(bpmBusinessProcess.BusinessNumber, bpmBusinessProcess.ProcInstId);

        // ????????????
        processInfoVo.ProcessTitle = bpmBusinessProcess.Description;

        // ?????????
        Employee? employee = _employeeInfoProviderService.QryLiteEmployeeInfoById(bpmBusinessProcess.CreateUser);
        processInfoVo.Employee = employee;
        processInfoVo.CreateTime = bpmBusinessProcess.CreateTime;

        // ???¡Â????? ID
        processInfoVo.StartUserId = bpmBusinessProcess.CreateUser;

        // ??????????
        processInfoVo.ProcessNumber = bpmBusinessProcess.BusinessNumber;

        string processInstanceId = bpmBusinessProcess.ProcInstId;

        // ??????????
        _processForwardService.UpdateProcessForward(new BpmProcessForward
        {
            ProcessInstanceId = processInstanceId, ForwardUserId = SecurityUtils.GetLogInEmpIdStr()
        });

        // ?????
        _userMessageService.ReadNode(processInstanceId);

        // ???????????????
        List<BpmAfTask>? tasks = _afTaskService
            .baseRepo
            .Where(a => a.ProcInstId == processInstanceId && a.Assignee == SecurityUtils.GetLogInEmpId())
            .ToList();
        string taskDefKey = "";

        if (tasks.Any())
        {
            BpmAfTask? firstTask = tasks.First();
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
        BpmBusinessProcess? bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(processCode);

        // ?????????????????????????????????????????????
        if (bpmBusinessProcess != null)
        {
            List<BpmAfTaskInst> taskInstanceList = _afTaskInstService
                .baseRepo
                .Where(a => a.ProcInstId == bpmBusinessProcess.ProcInstId)
                .ToList();


            List<string>? assigneeList = taskInstanceList
                .Where(task => task != null)
                .Select(task => task.Assignee)
                .ToList();

            if (assigneeList.Contains(SecurityUtils.GetLogInEmpIdStr()))
            {
                return true;
            }

            // TODO: ??????????
            return true;
        }

        return true;
    }
}