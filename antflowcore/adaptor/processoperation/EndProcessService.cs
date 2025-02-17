using antflowcore.bpmn;
using antflowcore.bpmn.service;
using antflowcore.constant.enus;
using AntFlowCore.Constants;
using antflowcore.entity;
using AntFlowCore.Entity;
using AntFlowCore.Enums;
using antflowcore.exception;
using antflowcore.factory;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace antflowcore.adaptor.processoperation;

/// <summary>
    /// End/Abort/Disagree a process
    /// </summary>
    public class EndProcessService : IProcessOperationAdaptor
    {
        private readonly FormFactory _formFactory;
        private readonly BpmBusinessProcessService _bpmBusinessProcessService;
        private readonly BpmVerifyInfoService _verifyInfoService;
        private readonly TaskMgmtService _taskMgmtService;
        private readonly ProcessBusinessContansService _businessConstants;
        private readonly TaskService _taskService;
        private readonly ThirdPartyCallBackService _thirdPartyCallBackService;
        private readonly ILogger<EndProcessService> _logger;

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
            var bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);

            string verifyUserName = string.Empty;
            string verifyUserId = string.Empty;

            if (vo.IsOutSideAccessProc!=null&&vo.IsOutSideAccessProc.Value)
            {
                if (vo.ObjectMap != null && vo.ObjectMap.Any())
                {
                    verifyUserName = vo.ObjectMap.ContainsKey("employeeName") ? vo.ObjectMap["employeeName"].ToString() : string.Empty;
                    verifyUserId = vo.ObjectMap.ContainsKey("employeeId") ? vo.ObjectMap["employeeId"].ToString() : string.Empty;
                }
            }
            else
            {
                verifyUserName = SecurityUtils.GetLogInEmpName();
                verifyUserId = SecurityUtils.GetLogInEmpIdStr();
            }



            var processInstanceId = bpmBusinessProcess.ProcInstId;
            var processState = (int)ProcessStateEnum.CRMCEL_STATE;

            if (vo.Flag!=null&&vo.Flag.Value)
            {
                processState = (int)ProcessStateEnum.END_STATE;
            }

            List<BpmAfTask> taskList = _taskService.CreateTaskQuery(a=>a.ProcInstId==processInstanceId&&a.Assignee==SecurityUtils.GetLogInEmpId());

            if (!taskList.Any())
            {
                throw new AFBizException("当前流程已审批!");
            }

            var taskData = taskList.First();

            // Update process state
            _bpmBusinessProcessService.UpdateBusinessProcess(new BpmBusinessProcess
            {
                BusinessNumber = bpmBusinessProcess.BusinessNumber,
                ProcessState = processState
            });

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
                RunInfoId = bpmBusinessProcess.ProcInstId
            });

            // Stop a process
            _businessConstants.DeleteProcessInstance(processInstanceId);

            // Call business adaptor method
            vo.BusinessId = bpmBusinessProcess.BusinessId;

            if (vo.IsOutSideAccessProc!=null&&vo.IsOutSideAccessProc.Value)
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