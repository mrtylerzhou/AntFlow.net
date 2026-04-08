using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Bpmn.service;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Common.util;
using AntFlowCore.Core.adaptor;
using AntFlowCore.Core.adaptor.processoperation;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Core.entity;
using AntFlowCore.Core.exception;
using AntFlowCore.Core.factory;
using AntFlowCore.Core.interf;
using AntFlowCore.Core.util;
using AntFlowCore.Enums;
using AntFlowCore.Persist.api.interf.repository;
using AntFlowCore.Vo;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

public class ResubmitProcessService: IProcessOperationAdaptor
{
        private readonly IFormFactory _formFactory;
        private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
        private readonly IBpmVerifyInfoService _verifyInfoService;
        private readonly ITaskService _taskService;
        private readonly IBpmProcessNodeSubmitBizService _processNodeSubmitBizService;
        private readonly IBpmVariableSignUpPersonnelService _bpmVariableSignUpPersonnelService;

        public ResubmitProcessService(
           IFormFactory formFactory,
           IBpmBusinessProcessService bpmBusinessProcessService,
           IBpmVerifyInfoService verifyInfoService,
           ITaskService taskService,
           IBpmProcessNodeSubmitBizService processNodeSubmitBizServiceService,
           IBpmVariableSignUpPersonnelService bpmVariableSignUpPersonnelService)
        {
            _formFactory = formFactory;
            _bpmBusinessProcessService = bpmBusinessProcessService;
            _verifyInfoService = verifyInfoService;
            _taskService = taskService;
            _processNodeSubmitBizService = processNodeSubmitBizServiceService;
            _bpmVariableSignUpPersonnelService = bpmVariableSignUpPersonnelService;
        }

        public void DoProcessButton(BusinessDataVo vo)
        {
            BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);
            vo.BusinessId = bpmBusinessProcess.BusinessId;

            //todo
            List<BpmAfTask> tasks = _taskService
                .CreateTaskQuery(t =>
                    t.ProcInstId == bpmBusinessProcess.ProcInstId);
                

            if (!tasks.Any())
            {
                throw new AFBizException("当前流程已审批！");
            }
            if (!tasks.Any(t => t.Assignee == SecurityUtils.GetLogInEmpIdStr()))
            {
                throw new AFBizException("当前流程已审批！");
            }
            BpmAfTask task = null;
            if (!string.IsNullOrEmpty(vo.TaskId))
            {
                task = tasks.First(t => t.Id == vo.TaskId);
            }
            else
            {
                task = tasks[0];
                if (string.IsNullOrEmpty(task.AssigneeName)) {
                    task.AssigneeName = SecurityUtils.GetLogInEmpName();
                }
            }

            if (task == null)
            {
                throw new AFBizException("当前流程代办已审批！");
            }

            vo.TaskId = task.Id;
            vo.TaskDefKey = task.TaskDefKey;
            task.ProcessNumber = bpmBusinessProcess.BusinessNumber;
            if (vo.IsOutSideAccessProc!=null&&!vo.IsOutSideAccessProc.Value)
            {
                _formFactory.GetFormAdaptor(vo).OnConsentData(vo);
            }

            BpmVerifyInfo bpmVerifyInfo = new BpmVerifyInfo
            {
                VerifyDate = DateTime.Now,
                TaskName = task.Name,
                TaskId = task.Id,
                RunInfoId = bpmBusinessProcess.ProcInstId,
                VerifyUserId = SecurityUtils.GetLogInEmpIdStr(),
                VerifyUserName = SecurityUtils.GetLogInEmpName(),
                TaskDefKey = task.TaskDefKey,
                VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
                VerifyDesc = string.IsNullOrEmpty(vo.ApprovalComment) ? "同意" : vo.ApprovalComment,
                ProcessCode = vo.ProcessNumber,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };

          
            if (vo != null && !string.IsNullOrEmpty(vo.ProcessDigest))
            {
                _bpmBusinessProcessService.Frsql
                    .Update<BpmBusinessProcess>()
                    .Set(a => a.ProcessDigest, vo.ProcessDigest)
                    .Where(a => a.BusinessNumber.Equals(vo.ProcessNumber))
                    .ExecuteAffrows();
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

            _processNodeSubmitBizService.ProcessComplete(task);
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