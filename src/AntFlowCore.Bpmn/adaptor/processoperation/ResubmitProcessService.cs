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
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity.jsonconf;
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
        private readonly ForwardToNodeService _forwardToNodeService;
        private readonly IBpmVariableService _bpmVariableService;
        private readonly IBpmnNodeService _bpmnNodeService;
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
           ForwardToNodeService forwardToNodeService,
           IBpmVariableService bpmVariableService,
           IBpmnNodeService bpmnNodeService,
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
            _forwardToNodeService = forwardToNodeService;
            _bpmVariableService = bpmVariableService;
            _bpmnNodeService = bpmnNodeService;
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

            // 同意推进节点:检查是否为同意推进节点,若是则完成当前任务后推进到目标节点
            if (TryApproveForward(vo, task, bpmBusinessProcess))
            {
                return;
            }



            _processNodeSubmitBizService.ProcessComplete(task);
        }

        /// <summary>
        /// 尝试执行"同意推进"逻辑。
        /// 检测task.FormKey中是否含有approve_forward_node标签,
        /// 若有则查询节点配置获取forwardNodeIds,完成当前任务后推进到目标节点。
        /// 对应 Java BpmProcessNodeSubmitBizServiceImpl.tryApproveForward.
        /// </summary>
        /// <returns>true=已处理推进, false=非同意推进节点,继续走原有审批逻辑</returns>
        private bool TryApproveForward(BusinessDataVo vo, BpmAfTask task, BpmBusinessProcess bpmBusinessProcess)
        {
            // 1. 快速短路:检查 formKey 是否含 approve_forward_node 标签
            string formKey = task.FormKey;
            if (string.IsNullOrEmpty(formKey))
            {
                return false;
            }

            NodeExtraInfoDTO? extraInfo;
            try
            {
                extraInfo = System.Text.Json.JsonSerializer.Deserialize<NodeExtraInfoDTO>(formKey,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                return false;
            }

            if (extraInfo?.NodeLabelVOS == null ||
                !NodeLabelConstants.NodeLabelContainsAny(extraInfo.NodeLabelVOS, StringConstants.APPROVE_FORWARD_NODE))
            {
                return false;
            }

            // 2. 查询当前节点配置获取 forwardNodeIds
            // 使用 GetNodeIdByElementId 直接获取 nodeId(避免集合方法的性能损耗)
            var nodeElement = _bpmVariableService.GetNodeIdByElementId(vo.ProcessNumber, task.TaskDefKey);
            if (nodeElement == null || string.IsNullOrEmpty(nodeElement.NodeId))
            {
                _logger.LogWarning("同意推进:未找到节点映射. processNumber={ProcessNumber}, taskDefKey={TaskDefKey}",
                    vo.ProcessNumber, task.TaskDefKey);
                return false;
            }

            long nodeId = long.Parse(nodeElement.NodeId);
            var bpmnNode = _bpmnNodeService._repository.FirstOrDefault(a => a.Id == nodeId);
            if (bpmnNode == null || string.IsNullOrEmpty(bpmnNode.NodeConfigJson))
            {
                _logger.LogWarning("同意推进:未找到节点配置. nodeId={NodeId}", nodeId);
                return false;
            }

            var configJson = JsonConfUtil.ParseNodeConfig(bpmnNode.NodeConfigJson);
            if (configJson?.ForwardNodeIds == null || configJson.ForwardNodeIds.Count == 0)
            {
                _logger.LogWarning("同意推进:节点配置缺少ForwardNodeIds. nodeId={NodeId}", nodeId);
                return false;
            }

            // 3. 使用 confId + node_id(UUID) 查询目标节点主键 id
            string targetNodeUuid = configJson.ForwardNodeIds[0];
            var targetNode = _bpmnNodeService._repository.FirstOrDefault(
                a => a.ConfId == bpmnNode.ConfId && a.NodeId == targetNodeUuid && a.IsDel == 0);
            if (targetNode == null)
            {
                _logger.LogWarning("同意推进:未找到目标节点. confId={ConfId}, targetNodeUuid={TargetNodeUuid}",
                    bpmnNode.ConfId, targetNodeUuid);
                return false;
            }

            // 4. 转换目标节点主键 id 为 elementId(taskDefKey)
            List<string> targetElementIds = _bpmVariableService.GetElementIdsdByNodeId(
                vo.ProcessNumber, targetNode.Id.ToString());
            if (targetElementIds == null || targetElementIds.Count == 0)
            {
                _logger.LogWarning("同意推进:未能根据nodeId获取目标节点taskDefKey. targetNodeId={TargetNodeId}",
                    targetNode.Id);
                return false;
            }
            string targetTaskDefKey = targetElementIds[0];

            // 5. 完成当前任务(同意)
            _taskService.Complete(task);

            // 6. 推进到目标节点(复用 ForwardToNodeService 的公共推进逻辑)
            _forwardToNodeService.MoveToTargetAfterComplete(
                bpmBusinessProcess.ProcInstId, vo.ProcessNumber, targetTaskDefKey,
                bpmBusinessProcess.ProcessinessKey);

            _logger.LogInformation("同意推进成功:processNumber={ProcessNumber}, targetTaskDefKey={TargetTaskDefKey}",
                vo.ProcessNumber, targetTaskDefKey);

            return true;
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