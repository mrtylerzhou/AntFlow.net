using AntFlow.Core.Bpmn.Service;
using AntFlow.Core.Constant;
using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Exception;
using AntFlow.Core.Factory;
using AntFlow.Core.Service.Business;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Adaptor.ProcessOperation;

/// <summary>
///     End/Abort/Disagree a process
/// </summary>
public class EndProcessService : IProcessOperationAdaptor
{
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly ProcessBusinessContansService _businessConstants;
    private readonly FormFactory _formFactory;
    private readonly ILogger<EndProcessService> _logger;
    private readonly TaskMgmtService _taskMgmtService;
    private readonly TaskService _taskService;
    private readonly ThirdPartyCallBackService _thirdPartyCallBackService;
    private readonly BpmVerifyInfoService _verifyInfoService;

    public EndProcessService(
        FormFactory formFactory,
        BpmBusinessProcessService bpmBusinessProcessService,
        BpmVerifyInfoService verifyInfoService,
        TaskService taskService,
        TaskMgmtService taskMgmtService,
        ProcessBusinessContansService businessConstants,
        ThirdPartyCallBackService thirdPartyCallBackService,
        ILogger<EndProcessService> logger)
    {
        _formFactory = formFactory;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _verifyInfoService = verifyInfoService;
        _taskMgmtService = taskMgmtService;

        _businessConstants = businessConstants;
        _taskService = taskService;
        _thirdPartyCallBackService = thirdPartyCallBackService;
        _logger = logger;
    }

    public void DoProcessButton(BusinessDataVo vo)
    {
        BpmBusinessProcess? bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);

        string verifyUserName = string.Empty;
        string verifyUserId = string.Empty;

        if (vo.IsOutSideAccessProc != null && vo.IsOutSideAccessProc.Value)
        {
            if (vo.ObjectMap != null && vo.ObjectMap.Any())
            {
                verifyUserName = vo.ObjectMap.ContainsKey("employeeName")
                    ? vo.ObjectMap["employeeName"].ToString()
                    : string.Empty;
                verifyUserId = vo.ObjectMap.ContainsKey("employeeId")
                    ? vo.ObjectMap["employeeId"].ToString()
                    : string.Empty;
            }
        }
        else
        {
            verifyUserName = SecurityUtils.GetLogInEmpName();
            verifyUserId = SecurityUtils.GetLogInEmpIdStr();
        }


        string? processInstanceId = bpmBusinessProcess.ProcInstId;
        int processState = (int)ProcessStateEnum.REJECT_STATE;

        if (vo.Flag != null && vo.Flag.Value)
        {
            processState = (int)ProcessStateEnum.END_STATE;
        }

        List<BpmAfTask> taskList = _taskService.CreateTaskQuery(a =>
            a.ProcInstId == processInstanceId && a.Assignee == SecurityUtils.GetLogInEmpId());

        if (!taskList.Any())
        {
            throw new AFBizException("当前流程已审批");
        }

        BpmAfTask? taskData = taskList.First();
        bpmBusinessProcess.ProcessState = processState;
        // Update process state
        _bpmBusinessProcessService.baseRepo
            .Update(bpmBusinessProcess);

        // Save verify info
        _verifyInfoService.AddVerifyInfo(new BpmVerifyInfo
        {
            BusinessId = bpmBusinessProcess.BusinessId,
            VerifyUserId = verifyUserId,
            VerifyUserName = verifyUserName,
            VerifyStatus = processState == (int)ProcessStateEnum.END_STATE
                ? (int)ProcessSubmitStateEnum.END_AGRESS_TYPE
                : processState,
            VerifyDate = DateTime.Now,
            ProcessCode = vo.ProcessNumber,
            VerifyDesc = vo.ApprovalComment,
            TaskName = taskData.Name,
            TaskId = taskData.Id,
            TaskDefKey = taskData.TaskDefKey,
            RunInfoId = bpmBusinessProcess.ProcInstId,
            TenantId = MultiTenantUtil.GetCurrentTenantId()
        });

        // Stop a process
        _businessConstants.DeleteProcessInstance(processInstanceId);

        // Call business adaptor method
        vo.BusinessId = bpmBusinessProcess.BusinessId;

        if (vo.IsOutSideAccessProc != null && vo.IsOutSideAccessProc.Value)
        {
            _formFactory.GetFormAdaptor(vo).OnCancellationData(vo);
        }
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(
            ProcessOperationEnum.BUTTON_TYPE_STOP,
            ProcessOperationEnum.BUTTON_TYPE_DIS_AGREE,
            ProcessOperationEnum.BUTTON_TYPE_ABANDON);

        ((IAdaptorService)this).AddSupportBusinessObjects(
            StringConstants.outSideAccessmarker,
            ProcessOperationEnum.BUTTON_TYPE_DIS_AGREE,
            ProcessOperationEnum.BUTTON_TYPE_ABANDON);
    }
}