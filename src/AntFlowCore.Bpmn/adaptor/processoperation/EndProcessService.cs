using AntFlowCore.Abstraction.Orm.util;
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
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity.jsonconf;
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
        private readonly BackToModifyService _backToModifyService;
        private readonly IBpmVariableService _bpmVariableService;
        private readonly IBpmnNodeService _bpmnNodeService;
        private readonly ILogger<EndProcessService> _logger;

        public EndProcessService(
            IFormFactory formFactory,
            IBpmBusinessProcessService bpmBusinessProcessService,
            IBpmVerifyInfoService verifyInfoService,
            TaskService taskService,
            ITaskMgmtService taskMgmtService,
            IProcessBusinessContansService businessConstants,
            IThirdPartyCallBackService thirdPartyCallBackService,
            BackToModifyService backToModifyService,
            IBpmVariableService bpmVariableService,
            IBpmnNodeService bpmnNodeService,
            ILogger<EndProcessService> logger)
        {
            _formFactory = formFactory;
            _bpmBusinessProcessService = bpmBusinessProcessService;
            _verifyInfoService = verifyInfoService;
            _taskMgmtService = taskMgmtService;
            
            _businessConstants = businessConstants;
            _taskService = taskService;
            _thirdPartyCallBackService = thirdPartyCallBackService;
            _backToModifyService = backToModifyService;
            _bpmVariableService = bpmVariableService;
            _bpmnNodeService = bpmnNodeService;
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

            // === 不同意退回分叉: 检测formKey中的disagree_back标签 ===
            if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_DIS_AGREE)
            {
                var currentTask = taskList.FirstOrDefault();
                if (currentTask != null && TryDisagreeBack(vo, currentTask, bpmBusinessProcess))
                {
                    return;
                }
            }

            var taskData = taskList.First();
            bpmBusinessProcess.ProcessState=processState;
            // Update process state
            _bpmBusinessProcessService._repository
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
            else
            {
                if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_DIS_AGREE)
                {
                    _formFactory.GetFormAdaptor(vo).OnDisagreeData(vo);
                }
            }
        }

        /// <summary>
        /// 尝试执行"不同意退回"逻辑。
        /// 检测task.FormKey中是否含有af_syslabel_disagree_back标签，
        /// 若有则查询节点配置获取backType和backToNodeId，转发给BackToModifyService处理。
        /// </summary>
        /// <returns>true=已转发处理, false=未配置退回,继续走原有结束逻辑</returns>
        private bool TryDisagreeBack(BusinessDataVo vo, BpmAfTask task, BpmBusinessProcess bpmBusinessProcess)
        {
            string formKey = task.FormKey;
            if (string.IsNullOrEmpty(formKey))
            {
                return false;
            }
            try
            {
                var extraInfo = System.Text.Json.JsonSerializer.Deserialize<NodeExtraInfoDTO>(formKey,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (extraInfo?.NodeLabelVOS == null ||
                    !NodeLabelConstants.NodeLabelContainsAny(extraInfo.NodeLabelVOS, StringConstants.AF_SYSLABEL_DISAGREE_BACK))
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "解析formKey失败,走默认结束流程");
                return false;
            }

            // 查询节点配置获取backType和backToNodeId
            var nodeElement = _bpmVariableService.GetNodeIdByElementId(vo.ProcessNumber, task.TaskDefKey);
            if (nodeElement == null || string.IsNullOrEmpty(nodeElement.NodeId))
            {
                _logger.LogWarning("未找到节点映射,走默认结束流程. processNumber={ProcessNumber}, taskDefKey={TaskDefKey}",
                    vo.ProcessNumber, task.TaskDefKey);
                return false;
            }

            long nodeId = long.Parse(nodeElement.NodeId);
            var bpmnNode = _bpmnNodeService._repository.FirstOrDefault(a => a.Id == nodeId);
            if (bpmnNode == null || string.IsNullOrEmpty(bpmnNode.NodeConfigJson))
            {
                _logger.LogWarning("未找到节点配置,走默认结束流程. nodeId={NodeId}", nodeId);
                return false;
            }

            var configJson = JsonConfUtil.ParseNodeConfig(bpmnNode.NodeConfigJson);
            int? backType = configJson?.BackType;
            string backToNodeId = configJson?.BackToNodeId;
            if (backType == null || (backType != 4 && backType != 5))
            {
                return false;
            }
            if (string.IsNullOrEmpty(backToNodeId))
            {
                _logger.LogError("不同意退回配置缺少目标节点! nodeId={NodeId}, backType={BackType}", nodeId, backType);
                throw new AFBizException("不同意退回配置缺少目标节点,请联系流程管理员!");
            }

            // 转发给BackToModifyService
            vo.BackToModifyType = backType;
            vo.BackToNodeId = backToNodeId;
            vo.TaskId = task.Id;
            _backToModifyService.DoProcessButton(vo);
            return true;
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

        /// <summary>
        /// Terminate the process WITHOUT recording a new verifyinfo entry.
        /// Used by OpposeProcessService when the oppose threshold M is reached:
        /// the oppose action already records its own verifyinfo (verify_status=7),
        /// so we only need to flip the process state and delete the process instance.
        /// Mirrors Java EndProcessImpl.endProcessWithoutVerify.
        /// </summary>
        /// <param name="vo">Business data carrying processNumber and (optionally) businessId.</param>
        public void EndProcessWithoutVerify(BusinessDataVo vo)
        {
            var bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);
            if (bpmBusinessProcess == null)
            {
                throw new AFBizException($"根据流程编号[{vo.ProcessNumber}]未找到流程实例");
            }

            // Mark process as rejected (REJECT_STATE=6) to reflect oppose-driven termination
            bpmBusinessProcess.ProcessState = (int)ProcessStateEnum.REJECT_STATE;
            _bpmBusinessProcessService._repository.Update(bpmBusinessProcess);

            string processInstanceId = bpmBusinessProcess.ProcInstId;
            _businessConstants.DeleteProcessInstance(processInstanceId);

            vo.BusinessId = bpmBusinessProcess.BusinessId;

            if (vo.IsOutSideAccessProc != null && vo.IsOutSideAccessProc.Value)
            {
                _formFactory.GetFormAdaptor(vo).OnCancellationData(vo);
            }
        }
        
    }