using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.processoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.service;
using AntFlowCore.Common.constant.enus;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Enums;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

/// <summary>
    /// End/Abort/Disagree a process
    /// </summary>
    public class EndProcessService : IProcessOperationAdaptor
    {
        private readonly IFormFactory _formFactory;
        private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
        private readonly IBpmVerifyInfoService _verifyInfoService;
        private readonly ITaskMgmtService _taskMgmtService;
        private readonly IProcessBusinessContansService _businessConstants;
        private readonly TaskService _taskService;
        private readonly IThirdPartyCallBackService _thirdPartyCallBackService;
        private readonly ILogger<EndProcessService> _logger;

        public EndProcessService(
            IFormFactory formFactory,
            IBpmBusinessProcessService bpmBusinessProcessService,
            IBpmVerifyInfoService verifyInfoService,
            TaskService taskService,
            ITaskMgmtService taskMgmtService,
            IProcessBusinessContansService businessConstants,
            IThirdPartyCallBackService thirdPartyCallBackService,
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
            var processState = (int)ProcessStateEnum.REJECT_STATE;

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
            bpmBusinessProcess.ProcessState=processState;
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
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
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