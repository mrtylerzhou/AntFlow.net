using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.adaptor;
using AntFlowCore.Base.adaptor.processoperation;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.interf;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.processoperation;

public class ResubmitProcessService: IProcessOperationAdaptor
{
        private readonly IFormFactory _formFactory;
        private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
        private readonly IBpmVerifyInfoService _verifyInfoService;
        private readonly ITaskService _taskService;
        private readonly IBpmProcessNodeSubmitBizService _processNodeSubmitBizService;
        private readonly IBpmVariableSignUpPersonnelService _bpmVariableSignUpPersonnelService;
        private readonly IBpmnConfBizService _bpmnConfBizService;
        private readonly IBpmProcessMigrationService _bpmProcessMigrationService;
        private readonly ILogger<ResubmitProcessService> _logger;

        public ResubmitProcessService(
           IFormFactory formFactory,
           IBpmBusinessProcessService bpmBusinessProcessService,
           IBpmVerifyInfoService verifyInfoService,
           ITaskService taskService,
           IBpmProcessNodeSubmitBizService processNodeSubmitBizServiceService,
           IBpmVariableSignUpPersonnelService bpmVariableSignUpPersonnelService,
           IBpmnConfBizService bpmnConfBizService,
           IBpmProcessMigrationService bpmProcessMigrationService,
           ILogger<ResubmitProcessService> logger)
        {
            _formFactory = formFactory;
            _bpmBusinessProcessService = bpmBusinessProcessService;
            _verifyInfoService = verifyInfoService;
            _taskService = taskService;
            _processNodeSubmitBizService = processNodeSubmitBizServiceService;
            _bpmVariableSignUpPersonnelService = bpmVariableSignUpPersonnelService;
            _bpmnConfBizService = bpmnConfBizService;
            _bpmProcessMigrationService = bpmProcessMigrationService;
            _logger = logger;
        }

        public void DoProcessButton(BusinessDataVo vo)
        {
            vo.StartUserId = SecurityUtils.GetLogInEmpIdStr();
            vo.StartUserName = SecurityUtils.GetLogInEmpName();

            BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService.GetBpmBusinessProcess(vo.ProcessNumber);
            vo.BusinessId = bpmBusinessProcess.BusinessId;

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

            BpmAfTask task;
            if (!string.IsNullOrEmpty(vo.TaskId))
            {
                task = tasks.First(t => t.Id == vo.TaskId && t.Assignee == SecurityUtils.GetLogInEmpIdStr());
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
                throw new AFBizException("当前流程代办已审批或不存在！");
            }

            // 动态条件检查:如果流程包含动态条件节点,且当前是最后一个审批人,
            // 检查条件是否发生变化.如果变化,则迁移流程(重新发起并推进到当前节点)
            if (ShouldCheckDynamicCondition(vo, task, tasks))
            {
                bool conditionsChanged = _bpmnConfBizService.MigrationCheckConditionsChange(vo);
                if (conditionsChanged)
                {
                    // 条件发生变化,执行流程迁移
                    _bpmProcessMigrationService.MigrateAndJumpToCurrent(task, bpmBusinessProcess, vo,
                        ExecuteTaskCompletion);
                    BpmVerifyInfoSupplement(vo, task, bpmBusinessProcess);
                    return;
                }
            }

            ExecuteTaskCompletion(vo, task, bpmBusinessProcess);
            vo.StartUserId = bpmBusinessProcess.CreateUser; // 为了发消息通知使用
        }

        /// <summary>
        /// 判断是否需要检查动态条件:
        /// 1. 只有当前节点到最后一个审批人时才执行检查(tasks.size==1)
        /// 2. 流程配置中包含 isDynamicCondition=true 的节点
        /// </summary>
        private bool ShouldCheckDynamicCondition(BusinessDataVo vo, BpmAfTask task, List<BpmAfTask> tasks)
        {
            // 只有当前节点到最后一个审批人了才执行迁移
            if (tasks.Count != 1)
            {
                return false;
            }

            // 检查流程配置中是否包含动态条件节点
            if (vo.BpmnConfVo?.Id != null && vo.BpmnConfVo.Id > 0)
            {
                BpmnConfVo bpmnConfVo = _bpmnConfBizService.Detail(vo.BpmnConfVo.Id);
                if (bpmnConfVo?.Nodes != null)
                {
                    foreach (var nodeVo in bpmnConfVo.Nodes)
                    {
                        if (nodeVo.IsDynamicCondition == true)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 操作记录补充(迁移时使用)
        /// </summary>
        private void BpmVerifyInfoSupplement(BusinessDataVo vo, BpmAfTask task, BpmBusinessProcess bpmBusinessProcess)
        {
            BpmVerifyInfo bpmVerifyInfo = new BpmVerifyInfo
            {
                VerifyDate = DateTime.Now,
                TaskName = task.Name,
                TaskId = task.Id,
                RunInfoId = bpmBusinessProcess.ProcInstId,
                VerifyUserId = task.Assignee,
                VerifyUserName = vo.StartUserName,
                TaskDefKey = task.TaskDefKey,
                VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
                VerifyDesc = string.IsNullOrEmpty(vo.ApprovalComment) ? "同意" : vo.ApprovalComment,
                ProcessCode = vo.ProcessNumber,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };

            if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_JP)
            {
                bpmVerifyInfo.VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_SIGN_UP;
                bpmVerifyInfo.VerifyDesc = string.IsNullOrEmpty(vo.ApprovalComment) ? "加批" : vo.ApprovalComment;
            }

            if (!StringConstants.CURRENT_USER_ALREADY_PROCESSED.Equals(bpmVerifyInfo.VerifyDesc))
            {
                _verifyInfoService.AddVerifyInfo(bpmVerifyInfo);
            }
        }

        private void ExecuteTaskCompletion(BusinessDataVo vo, BpmAfTask task, BpmBusinessProcess bpmBusinessProcess)
        {
            vo.TaskId = task.Id;

            if (vo.IsOutSideAccessProc == null || !vo.IsOutSideAccessProc.Value)
            {
                _formFactory.GetFormAdaptor(vo).OnConsentData(vo);
            }

            BpmVerifyInfo bpmVerifyInfo = new BpmVerifyInfo
            {
                VerifyDate = DateTime.Now,
                TaskName = task.Name,
                TaskId = task.Id,
                RunInfoId = bpmBusinessProcess.ProcInstId,
                VerifyUserId = task.Assignee,
                VerifyUserName = vo.StartUserName,
                TaskDefKey = task.TaskDefKey,
                VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
                VerifyDesc = string.IsNullOrEmpty(vo.ApprovalComment) ? "同意" : vo.ApprovalComment,
                ProcessCode = vo.ProcessNumber,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };

            if (vo != null && !string.IsNullOrEmpty(vo.ProcessDigest))
            {
                _bpmBusinessProcessService._repository.UpdateProcessDigest(vo.ProcessNumber, vo.ProcessDigest);
            }

            if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_JP)
            {
                bpmVerifyInfo.VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_SIGN_UP;
                bpmVerifyInfo.VerifyDesc = string.IsNullOrEmpty(vo.ApprovalComment) ? "加批" : vo.ApprovalComment;
            }

            if (!StringConstants.CURRENT_USER_ALREADY_PROCESSED.Equals(bpmVerifyInfo.VerifyDesc))
            {
                _verifyInfoService.AddVerifyInfo(bpmVerifyInfo);
            }

            if (vo.OperationType == (int)ProcessOperationEnum.BUTTON_TYPE_JP)
            {
                _bpmVariableSignUpPersonnelService.InsertSignUpPersonnel(
                    vo.ProcessNumber, task.TaskDefKey, task.Assignee, vo.SignUpUsers);
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