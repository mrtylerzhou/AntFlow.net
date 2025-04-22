using antflowcore.bpmn.service;
using antflowcore.constant.enums;
using antflowcore.entity;
using antflowcore.exception;
using antflowcore.factory;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Entity;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.processoperation;

public class ResubmitProcessService : IProcessOperationAdaptor
{
    private readonly FormFactory _formFactory;
    private readonly BpmBusinessProcessService _bpmBusinessProcessService;
    private readonly BpmVerifyInfoService _verifyInfoService;
    private readonly TaskService _taskService;
    private readonly BpmProcessNodeSubmitService _processNodeSubmitService;
    private readonly BpmVariableSignUpPersonnelService _bpmVariableSignUpPersonnelService;

    public ResubmitProcessService(
        FormFactory formFactory,
        BpmBusinessProcessService bpmBusinessProcessService,
        BpmVerifyInfoService verifyInfoService,
        TaskService taskService,
        BpmProcessNodeSubmitService processNodeSubmitService,
        BpmVariableSignUpPersonnelService bpmVariableSignUpPersonnelService)
    {
        _formFactory = formFactory;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _verifyInfoService = verifyInfoService;
        _taskService = taskService;
        _processNodeSubmitService = processNodeSubmitService;
        _bpmVariableSignUpPersonnelService = bpmVariableSignUpPersonnelService;
    }

    public void DoProcessButton(BusinessDataVo vo)
    {
        var bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);
        vo.BusinessId = bpmBusinessProcess.BusinessId;

        //todo
        var tasks = _taskService
            .CreateTaskQuery(t =>
                t.ProcInstId == bpmBusinessProcess.ProcInstId &&
                t.Assignee == SecurityUtils.GetLogInEmpIdStr());

        if (!tasks.Any())
        {
            throw new AFBizException("当前流程已审批！");
        }

        BpmAfTask task = null;
        if (!string.IsNullOrEmpty(vo.TaskId))
        {
            task = tasks.FirstOrDefault();
        }
        else
        {
            task = tasks.First();
        }

        if (task == null)
        {
            throw new AFBizException("当前流程代办已审批！");
        }

        vo.TaskId = task.Id;
        task.ProcessNumber = bpmBusinessProcess.BusinessNumber;
        if (vo.IsOutSideAccessProc != null && !vo.IsOutSideAccessProc.Value)
        {
            _formFactory.GetFormAdaptor(vo).OnConsentData(vo);
        }

        var bpmVerifyInfo = new BpmVerifyInfo
        {
            VerifyDate = DateTime.Now,
            TaskName = task.Name,
            TaskId = task.Id,
            RunInfoId = bpmBusinessProcess.ProcInstId,
            VerifyUserId = SecurityUtils.GetLogInEmpIdStr(),
            VerifyUserName = SecurityUtils.GetLogInEmpName(),
            VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
            VerifyDesc = string.IsNullOrEmpty(vo.ApprovalComment) ? "同意" : vo.ApprovalComment,
            ProcessCode = vo.ProcessNumber
        };

        if (vo != null && !string.IsNullOrEmpty(vo.ProcessDigest))
        {
            _bpmBusinessProcessService.Update(new BpmBusinessProcess
            {
                ProcessDigest = vo.ProcessDigest
            });
        }

        if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_JP)
        {
            bpmVerifyInfo.VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_SIGN_UP;
            bpmVerifyInfo.VerifyDesc = string.IsNullOrEmpty(vo.ApprovalComment) ? "加批" : vo.ApprovalComment;
        }

        _verifyInfoService.AddVerifyInfo(bpmVerifyInfo);

        if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_JP)
        {
            _bpmVariableSignUpPersonnelService.InsertSignUpPersonnel(
                _taskService, task.Id, vo.ProcessNumber, task.TaskDefKey, task.Assignee, vo.SignUpUsers);
        }

        _processNodeSubmitService.ProcessComplete(task);
    }

    public void SetSupportBusinessObjects()
    {
        ((IAdaptorService)this).AddSupportBusinessObjects(ProcessOperationEnum.BUTTON_TYPE_RESUBMIT,
            ProcessOperationEnum.BUTTON_TYPE_AGREE,
            ProcessOperationEnum.BUTTON_TYPE_JP);
        ((IAdaptorService)this).AddSupportBusinessObjects(StringConstants.outSideAccessmarker,
            ProcessOperationEnum.BUTTON_TYPE_RESUBMIT,
            ProcessOperationEnum.BUTTON_TYPE_AGREE,
            ProcessOperationEnum.BUTTON_TYPE_JP);
    }
}