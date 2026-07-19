using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.formatter.filter;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.factory;
using AntFlowCore.Base.interf;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.util;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AntFlowCore.Bpmn.listener;

public class BpmnTaskListener: ITaskListener
{
    private readonly IBpmnConfService _bpmnConfService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IBpmProcessForwardService _bpmProcessForwardService;
    private readonly IUserEntrustService _userEntrustService;
    private readonly IBpmVariableMessageListenerService _bpmVariableMessageListenerService;
    private readonly IProcessBusinessContansService _processBusinessContansService;
    private readonly IBpmVerifyInfoService _bpmVerifyInfoService;
    private readonly ILogger<BpmnTaskListener> _logger;

    public BpmnTaskListener(
        IBpmnConfService bpmnConfService,
       IBpmBusinessProcessService bpmBusinessProcessService,
       IBpmProcessForwardService bpmProcessForwardService,
       IUserEntrustService userEntrustService,
       IBpmVariableMessageListenerService bpmVariableMessageListenerService,
       IProcessBusinessContansService processBusinessContansService,
       IBpmVerifyInfoService bpmVerifyInfoService,
        ILogger<BpmnTaskListener> logger)
    {
        _bpmnConfService = bpmnConfService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _bpmProcessForwardService = bpmProcessForwardService;
        _userEntrustService = userEntrustService;
        _bpmVariableMessageListenerService = bpmVariableMessageListenerService;
        _processBusinessContansService = processBusinessContansService;
        _bpmVerifyInfoService = bpmVerifyInfoService;
        _logger = logger;
    }
   
    public void Notify(BpmAfTask delegateTask,string eventName)
    {
        if(delegateTask.NodeType==(int)NodeTypeEnum.NODE_TYPE_COPY)
        {
            BpmProcessForward bpmProcessForward = new BpmProcessForward()
            {
                CreateUserId = SecurityUtils.GetLogInEmpIdStr(),
                ForwardUserId = delegateTask.Assignee,
                ForwardUserName = delegateTask.AssigneeName,
                ProcessNumber = delegateTask.ProcessNumber,
                ProcessInstanceId = delegateTask.ProcInstId,
                IsRead = 0,
                CreateTime = DateTime.Now,
            };
            _bpmProcessForwardService.AddProcessForward(bpmProcessForward);
            delegateTask.Assignee = AFSpecialAssigneeEnum.COPY_NODE.Id;
            delegateTask.AssigneeName = AFSpecialAssigneeEnum.COPY_NODE.Desc;
           var taskService = ServiceProviderUtils.GetService<ITaskService>();
           taskService.Complete(delegateTask);
           return;
        }

        // 抄送节点v2:节点以普通审批人身份进入引擎,通过标签识别后自动完成
        if (ProcessCopyV2(delegateTask))
        {
            return;
        }

        // 条件抄送节点:总是自动完成,仅条件满足时写抄送记录
        if (ProcessConditionCopyNode(delegateTask))
        {
            return;
        }

        // 条件审批节点:条件满足时自动通过,否则等人工审批
        if (ProcessConditionApproveNode(delegateTask))
        {
            return;
        }

        // Adjacent deduplication auto-skip: check FormKey for skippedAssignees label
        ProcessSkippedAssignee(delegateTask);

        BpmBusinessProcess bpmBusinessProcess = _bpmBusinessProcessService._repository
            .Find(a=>a.BusinessNumber==delegateTask.ProcessNumber)
            .First();
        if (bpmBusinessProcess == null)
        {
            _logger.LogError("流程实例不存在，流程号：{}", delegateTask.ProcessNumber);
            throw new AFBizException($"流程实例不存在，流程号：{delegateTask.ProcessNumber}");
        }
        BpmnConf bpmnConf = _bpmnConfService._repository
            .Find(a => a.BpmnCode == bpmBusinessProcess.Version)
            .FirstOrDefault();
        if (bpmnConf == null)
        {
            _logger.LogError("流程配置不存在，流程号：{}", delegateTask.ProcessNumber);
            throw new AFBizException($"流程配置不存在，流程号：{delegateTask.ProcessNumber}");
        }

        string formCode = bpmBusinessProcess.ProcessinessKey;
        bool isOutside=(bpmnConf.IsOutSideProcess??0)==1;
        string processNumber = bpmBusinessProcess.BusinessNumber;
        string bpmnCode = bpmnConf.BpmnCode;
        BpmVariableMessageVo bpmVariableMessageVo = new BpmVariableMessageVo
        {
            ProcessNumber = processNumber,
            FormCode = formCode,
            EventType=(int)EventTypeEnum.PROCESS_FLOW,
            MessageType = EventTypeEnum.PROCESS_FLOW.IsInNode()?2:1,
            ElementId = delegateTask.TaskDefKey,
            Assignee = delegateTask.Assignee,
            EventTypeEnum = EventTypeEnum.PROCESS_FLOW,
            Type = 2,
        };

        string bpmnConfConfConfigJson = bpmnConf.ConfConfigJson;
        if (string.IsNullOrEmpty(bpmnConfConfConfigJson))
        {
            return;
        }
        BpmnConfConfigJson? bpmnConfConfigJson = JsonConfUtil.ParseConfConfig(bpmnConfConfConfigJson);
        List<int>? noticeChannelTypes = bpmnConfConfigJson.NoticeChannelTypes;
        if (noticeChannelTypes.IsEmpty())
        {
            return ;
        }
        bool sendByTemplate = _bpmVariableMessageListenerService.ListenerCheckIsSendByTemplate(bpmVariableMessageVo);
        if (sendByTemplate)
        {
            //set is outside
            bpmVariableMessageVo.IsOutside = isOutside;

            //set template message
            _bpmVariableMessageListenerService.ListenerSendTemplateMessages(bpmVariableMessageVo);
        }
        else
        {
            ProcessInforVo processInforVo = new ProcessInforVo
            {
                ProcessinessKey = bpmnCode,
                BusinessNumber = processNumber,
                FormCode = formCode,
                Type = 2,
            };
            string emailUrl = _processBusinessContansService.GetRoute(ProcessNoticeEnum.EMAIL_TYPE.Code, processInforVo , isOutside);
            string appUrl = _processBusinessContansService.GetRoute(ProcessNoticeEnum.APP_TYPE.Code, processInforVo , isOutside);
            ActivitiBpmMsgVo activitiBpmMsgVo = new ActivitiBpmMsgVo
            {
                UserId = delegateTask.Assignee,
                ProcessId = processNumber,
                BpmnCode = bpmnCode,
                FormCode = formCode,
                ProcessName = bpmnConf.BpmnName,
                EmailUrl = emailUrl,
                Url = emailUrl,
                AppPushUrl = appUrl,
                TaskId = delegateTask.ProcInstId,
            };
            ActivitiTemplateMsgUtils.sendBpmApprovalMsg(activitiBpmMsgVo);
        }
    }

    /// <summary>
    /// 相邻节点去重自动跳过:检查任务的FormKey中是否包含skippedAssignees标签,
    /// 如果当前任务审批人在标签的labelName(逗号分隔的审批人ID列表)中,
    /// 则自动完成任务并记录审批信息.
    /// </summary>
    private void ProcessSkippedAssignee(BpmAfTask delegateTask)
    {
        string formKey = delegateTask.FormKey;
        if (string.IsNullOrEmpty(formKey) || !formKey.StartsWith("{"))
        {
            return;
        }

        NodeExtraInfoDTO? extraInfoDTO = null;
        try
        {
            extraInfoDTO = JsonSerializer.Deserialize<NodeExtraInfoDTO>(formKey);
        }
        catch
        {
            return;
        }

        if (extraInfoDTO?.NodeLabelVOS == null || extraInfoDTO.NodeLabelVOS.Count == 0)
        {
            return;
        }

        foreach (var nodeLabelVO in extraInfoDTO.NodeLabelVOS)
        {
            if (!StringConstants.SKIPPED_ASSIGNEE.Equals(nodeLabelVO.LabelValue))
            {
                continue;
            }

            string labelName = nodeLabelVO.LabelName;
            if (string.IsNullOrEmpty(labelName))
            {
                continue;
            }

            var skippedAssigneeIds = labelName.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
            var currentSkippedAssignee = skippedAssigneeIds.Where(a => a.Contains(delegateTask.Assignee)).ToList();
            if (currentSkippedAssignee.Count > 0)
            {
                // auto-complete the task
                var taskService = ServiceProviderUtils.GetService<ITaskService>();
                taskService?.Complete(delegateTask);

                // save verify info
                BpmBusinessProcess? bpmBusinessProcess = _bpmBusinessProcessService._repository
                    .Find(a => a.BusinessNumber == delegateTask.ProcessNumber)
                    .FirstOrDefault();
                if (bpmBusinessProcess != null)
                {
                    BpmVerifyInfo bpmVerifyInfo = new BpmVerifyInfo
                    {
                        VerifyDate = DateTime.Now,
                        TaskName = delegateTask.Name,
                        TaskId = delegateTask.Id,
                        RunInfoId = bpmBusinessProcess.ProcInstId,
                        VerifyUserId = delegateTask.Assignee,
                        VerifyUserName = delegateTask.AssigneeName,
                        TaskDefKey = delegateTask.TaskDefKey,
                        VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
                        VerifyDesc = StringConstants.AF_AUTO_SKIP_COMMENT,
                        ProcessCode = delegateTask.ProcessNumber,
                        TenantId = MultiTenantUtil.GetCurrentTenantId(),
                    };
                    _bpmVerifyInfoService.AddVerifyInfo(bpmVerifyInfo);
                }
                return;
            }
        }
    }

    /// <summary>
    /// 抄送节点v2运行时处理:当任务带有 copyNodeV2 标签时,将该任务标记为抄送,
    /// 自动完成任务,并记录抄送信息和审批日志.
    /// 对应 Java NextNodeLabelsProcessor.processCopyV2.
    /// </summary>
    /// <returns>true 表示任务是抄送节点v2并已自动完成;false 表示不是</returns>
    private bool ProcessCopyV2(BpmAfTask delegateTask)
    {
        string formKey = delegateTask.FormKey;
        if (string.IsNullOrEmpty(formKey) || !formKey.StartsWith("{"))
        {
            return false;
        }

        NodeExtraInfoDTO? extraInfoDTO = null;
        try
        {
            extraInfoDTO = JsonSerializer.Deserialize<NodeExtraInfoDTO>(formKey);
        }
        catch
        {
            return false;
        }

        if (extraInfoDTO?.NodeLabelVOS == null || extraInfoDTO.NodeLabelVOS.Count == 0)
        {
            return false;
        }

        bool isCopyV2 = false;
        foreach (var nodeLabelVO in extraInfoDTO.NodeLabelVOS)
        {
            if (StringConstants.COPY_NODEV2.Equals(nodeLabelVO.LabelValue))
            {
                isCopyV2 = true;
                break;
            }
        }

        if (!isCopyV2)
        {
            return false;
        }

        string procInstId = delegateTask.ProcInstId;
        string processNumber = delegateTask.ProcessNumber;
        string assignee = delegateTask.Assignee;
        string assigneeName = delegateTask.AssigneeName;

        // 检查是否已存在抄送记录,避免重复添加
        var existingForwards = _bpmProcessForwardService._repository
            .Find(a => a.ProcessInstanceId == procInstId && a.ForwardUserId == assignee);

        // 设置任务审批人为抄送人特殊标记,并自动完成任务
        delegateTask.Assignee = AFSpecialAssigneeEnum.CC_NODE.Id;
        string ccAssigneeName = AFSpecialAssigneeEnum.CC_NODE.Desc + "(" + assigneeName + ")";
        delegateTask.AssigneeName = ccAssigneeName;

        var taskService = ServiceProviderUtils.GetService<ITaskService>();
        taskService?.Complete(delegateTask);

        // 若不存在抄送记录,则新增
        if (existingForwards == null || existingForwards.Count == 0)
        {
            BpmProcessForward bpmProcessForward = new BpmProcessForward
            {
                CreateTime = DateTime.Now,
                CreateUserId = assignee,
                ForwardUserId = assignee,
                ForwardUserName = assigneeName,
                ProcessInstanceId = procInstId,
                ProcessNumber = processNumber,
                IsRead = 0,
                IsDel = 0,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };
            _bpmProcessForwardService.AddProcessForward(bpmProcessForward);
        }

        // 记录审批日志:(抄送给xxx)自动通过
        BpmVerifyInfo bpmVerifyInfo = new BpmVerifyInfo
        {
            VerifyDate = DateTime.Now,
            TaskName = delegateTask.Name,
            TaskId = delegateTask.Id,
            RunInfoId = procInstId,
            VerifyUserId = delegateTask.Assignee,
            VerifyUserName = ccAssigneeName,
            TaskDefKey = delegateTask.TaskDefKey,
            VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
            VerifyDesc = "(抄送给" + assigneeName + ")自动通过",
            ProcessCode = processNumber,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };
        _bpmVerifyInfoService.AddVerifyInfo(bpmVerifyInfo);

        return true;
    }

    /// <summary>
    /// 条件审批节点运行时处理:当任务带有 condition_approve_node 标签时,
    /// 评估条件.条件为 true 则自动完成任务(写 verifyInfo);条件为 false/null 则不处理,
    /// 让流程继续到正常的人工审批逻辑.
    /// 对应 Java NextNodeLabelsProcessor.processConditionApproveNode.
    /// </summary>
    /// <returns>true 表示已自动完成;false 表示不是条件审批节点或条件未满足</returns>
    private bool ProcessConditionApproveNode(BpmAfTask delegateTask)
    {
        string formKey = delegateTask.FormKey;
        if (string.IsNullOrEmpty(formKey) || !formKey.StartsWith("{"))
        {
            return false;
        }

        NodeExtraInfoDTO? extraInfoDTO = null;
        try
        {
            extraInfoDTO = JsonSerializer.Deserialize<NodeExtraInfoDTO>(formKey);
        }
        catch
        {
            return false;
        }

        if (extraInfoDTO?.NodeLabelVOS == null || extraInfoDTO.NodeLabelVOS.Count == 0)
        {
            return false;
        }

        bool isConditionApprove = false;
        foreach (var nodeLabelVO in extraInfoDTO.NodeLabelVOS)
        {
            if (StringConstants.CONDITION_APPROVE_NODE.Equals(nodeLabelVO.LabelValue))
            {
                isConditionApprove = true;
                break;
            }
        }

        if (!isConditionApprove)
        {
            return false;
        }

        // 评估条件
        bool? conditionResult = EvaluateCondition(delegateTask);

        if (conditionResult != true)
        {
            // 条件未满足,等待人工审批,不写任何记录,让流程继续
            _logger.LogInformation("条件审批节点条件未满足,等待人工审批. procInstId={}, taskId={}",
                delegateTask.ProcInstId, delegateTask.Id);
            return false;
        }

        // 条件满足,自动完成任务
        var taskService = ServiceProviderUtils.GetService<ITaskService>();
        taskService?.Complete(delegateTask);

        // 写审批日志
        BpmBusinessProcess? bpmBusinessProcess = _bpmBusinessProcessService._repository
            .Find(a => a.BusinessNumber == delegateTask.ProcessNumber)
            .FirstOrDefault();
        if (bpmBusinessProcess != null)
        {
            BpmVerifyInfo bpmVerifyInfo = new BpmVerifyInfo
            {
                VerifyDate = DateTime.Now,
                TaskName = delegateTask.Name,
                TaskId = delegateTask.Id,
                RunInfoId = bpmBusinessProcess.ProcInstId,
                VerifyUserId = delegateTask.Assignee,
                VerifyUserName = delegateTask.AssigneeName,
                TaskDefKey = delegateTask.TaskDefKey,
                VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
                VerifyDesc = StringConstants.AF_CONDITION_APPROVE_AUTO_COMMENT,
                ProcessCode = delegateTask.ProcessNumber,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };
            _bpmVerifyInfoService.AddVerifyInfo(bpmVerifyInfo);
        }

        return true;
    }

    /// <summary>
    /// 条件抄送节点运行时处理:当任务带有 condition_copy_node 标签时,
    /// 总是自动完成任务(设 CC_NODE 虚拟审批人);仅条件为 true 时写 BpmProcessForward 抄送记录.
    /// 对应 Java NextNodeLabelsProcessor.processConditionCopyNode.
    /// </summary>
    /// <returns>true 表示是条件抄送节点并已自动完成;false 表示不是</returns>
    private bool ProcessConditionCopyNode(BpmAfTask delegateTask)
    {
        string formKey = delegateTask.FormKey;
        if (string.IsNullOrEmpty(formKey) || !formKey.StartsWith("{"))
        {
            return false;
        }

        NodeExtraInfoDTO? extraInfoDTO = null;
        try
        {
            extraInfoDTO = JsonSerializer.Deserialize<NodeExtraInfoDTO>(formKey);
        }
        catch
        {
            return false;
        }

        if (extraInfoDTO?.NodeLabelVOS == null || extraInfoDTO.NodeLabelVOS.Count == 0)
        {
            return false;
        }

        bool isConditionCopy = false;
        foreach (var nodeLabelVO in extraInfoDTO.NodeLabelVOS)
        {
            if (StringConstants.CONDITION_COPY_NODE.Equals(nodeLabelVO.LabelValue))
            {
                isConditionCopy = true;
                break;
            }
        }

        if (!isConditionCopy)
        {
            return false;
        }

        string procInstId = delegateTask.ProcInstId;
        string processNumber = delegateTask.ProcessNumber;
        string assignee = delegateTask.Assignee;
        string assigneeName = delegateTask.AssigneeName;

        // 评估条件
        bool? conditionResult = EvaluateCondition(delegateTask);

        // 设置任务审批人为抄送人特殊标记,并自动完成任务(无论条件如何)
        delegateTask.Assignee = AFSpecialAssigneeEnum.CC_NODE.Id;
        string ccAssigneeName = AFSpecialAssigneeEnum.CC_NODE.Desc + "(" + assigneeName + ")";
        delegateTask.AssigneeName = ccAssigneeName;

        var taskService = ServiceProviderUtils.GetService<ITaskService>();
        taskService?.Complete(delegateTask);

        // 仅条件满足时写抄送记录
        if (conditionResult == true)
        {
            var existingForwards = _bpmProcessForwardService._repository
                .Find(a => a.ProcessInstanceId == procInstId && a.ForwardUserId == assignee);
            if (existingForwards == null || existingForwards.Count == 0)
            {
                BpmProcessForward bpmProcessForward = new BpmProcessForward
                {
                    CreateTime = DateTime.Now,
                    CreateUserId = assignee,
                    ForwardUserId = assignee,
                    ForwardUserName = assigneeName,
                    ProcessInstanceId = procInstId,
                    ProcessNumber = processNumber,
                    IsRead = 0,
                    IsDel = 0,
                    TenantId = MultiTenantUtil.GetCurrentTenantId(),
                };
                _bpmProcessForwardService.AddProcessForward(bpmProcessForward);
            }
        }

        // 记录审批日志:条件满足时为"执行抄送",不满足时为"跳过抄送"
        BpmVerifyInfo bpmVerifyInfo = new BpmVerifyInfo
        {
            VerifyDate = DateTime.Now,
            TaskName = delegateTask.Name,
            TaskId = delegateTask.Id,
            RunInfoId = procInstId,
            VerifyUserId = delegateTask.Assignee,
            VerifyUserName = ccAssigneeName,
            TaskDefKey = delegateTask.TaskDefKey,
            VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
            VerifyDesc = conditionResult == true
                ? StringConstants.AF_CONDITION_COPY_EXECUTE_COMMENT
                : StringConstants.AF_CONDITION_COPY_SKIP_COMMENT,
            ProcessCode = processNumber,
            TenantId = MultiTenantUtil.GetCurrentTenantId(),
        };
        _bpmVerifyInfoService.AddVerifyInfo(bpmVerifyInfo);

        return true;
    }

    /// <summary>
    /// 条件评估辅助方法:从 delegateTask 找到对应的 BpmnNode 配置,
    /// 取出 autoNodeConf 中的 conditionList,转成 BpmnNodeConditionsConfBaseVo,
    /// 调用 IConditionService.CheckMatchCondition 进行评估.
    /// </summary>
    /// <returns>true=条件满足;false=条件不满足;null=评估失败(找不到节点或配置)</returns>
    private bool? EvaluateCondition(BpmAfTask delegateTask)
    {
        // 按需解析依赖,避免构造期循环依赖
        var conditionService = ServiceProviderUtils.GetService<IConditionService>();
        var formFactory = ServiceProviderUtils.GetService<IFormFactory>();
        var bpmVariableBizService = ServiceProviderUtils.GetService<IBpmvariableBizService>();
        var bpmnNodeService = ServiceProviderUtils.GetService<IBpmnNodeService>();

        if (conditionService == null || formFactory == null || bpmVariableBizService == null || bpmnNodeService == null)
        {
            _logger.LogError("条件评估失败:必要服务未注册");
            return null;
        }

        try
        {
            // 1. 通过 processNumber 找到 BpmBusinessProcess
            BpmBusinessProcess? bpmBusinessProcess = _bpmBusinessProcessService._repository
                .Find(a => a.BusinessNumber == delegateTask.ProcessNumber)
                .FirstOrDefault();
            if (bpmBusinessProcess == null)
            {
                _logger.LogError("条件评估失败:流程实例不存在,流程号={}", delegateTask.ProcessNumber);
                return null;
            }

            // 2. 通过 bpmnCode 找到 BpmnConf
            BpmnConf? bpmnConf = _bpmnConfService._repository
                .Find(a => a.BpmnCode == bpmBusinessProcess.Version)
                .FirstOrDefault();
            if (bpmnConf == null)
            {
                _logger.LogError("条件评估失败:流程配置不存在,流程号={}", delegateTask.ProcessNumber);
                return null;
            }

            // 3. 通过 elementId 找到 nodeId,再找到 BpmnNode
            string elementId = delegateTask.TaskDefKey;
            NodeElementDto? nodeElementDto = bpmVariableBizService.GetNodeIdByElementId(delegateTask.ProcessNumber, elementId);
            if (nodeElementDto == null || string.IsNullOrEmpty(nodeElementDto.NodeId))
            {
                _logger.LogError("条件评估失败:无法根据 elementId 找到 nodeId,processNumber={}, elementId={}",
                    delegateTask.ProcessNumber, elementId);
                return null;
            }

            BpmnNode? bpmnNode = bpmnNodeService._repository
                .Find(a => a.ConfId == bpmnConf.Id && a.NodeId == nodeElementDto.NodeId)
                .FirstOrDefault();
            if (bpmnNode == null)
            {
                _logger.LogError("条件评估失败:找不到 BpmnNode,confId={}, nodeId={}", bpmnConf.Id, nodeElementDto.NodeId);
                return null;
            }

            // 4. 解析 NodeConfigJson,取出 autoNodeConf
            BpmnNodeConfigJson? nodeConfig = JsonConfUtil.ParseNodeConfig(bpmnNode.NodeConfigJson);
            if (nodeConfig?.AutoNodeConf == null)
            {
                _logger.LogWarning("条件评估失败:节点未配置 autoNodeConf,nodeId={}", nodeElementDto.NodeId);
                return null;
            }

            // 5. 把 autoNodeConf.ConditionList 转换为 BpmnNodeConditionsConfBaseVo
            //    复用 BpmnConfNodePropertyConverter.FromVue3Model,需要构造 BpmnNodePropertysVo
            BpmnNodePropertysVo propertysVo = new BpmnNodePropertysVo
            {
                ConditionList = nodeConfig.AutoNodeConf.ConditionList?
                    .Select(group => group?
                        .Select(item => item.ValueKind == JsonValueKind.Null
                            ? null
                            : JsonSerializer.Deserialize<BpmnNodeConditionsConfVueVo>(item.GetRawText()))
                        .Where(x => x != null)
                        .Select(x => x!)
                        .ToList() ?? new List<BpmnNodeConditionsConfVueVo>())
                    .ToList() ?? new List<List<BpmnNodeConditionsConfVueVo>>(),
                GroupRelation = nodeConfig.AutoNodeConf.GroupRelation ?? false,
            };

            BpmnNodeConditionsConfBaseVo conditionsConf;
            try
            {
                conditionsConf = BpmnConfNodePropertyConverter.FromVue3Model(propertysVo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "条件评估失败:conditionList 转换失败,nodeId={}", nodeElementDto.NodeId);
                return null;
            }

            // 6. 构造 BpmnNodeVo(只填必要字段)
            BpmnNodeVo nodeVo = new BpmnNodeVo
            {
                NodeId = nodeElementDto.NodeId,
                NodeType = (int)NodeTypeEnum.NODE_TYPE_APPROVER,
            };

            // 7. 构造 BusinessDataVo,加载业务数据,得到 BpmnStartConditionsVo
            string formCode = bpmBusinessProcess.ProcessinessKey;
            BusinessDataVo businessDataVo = new BusinessDataVo
            {
                FormCode = formCode,
                ProcessNumber = delegateTask.ProcessNumber,
                BusinessId = bpmBusinessProcess.BusinessId,
                BpmnCode = bpmBusinessProcess.Version,
            };

            try
            {
                var formAdaptor = formFactory.GetFormAdaptor(businessDataVo);
                formAdaptor.OnQueryData(businessDataVo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "条件评估失败:加载业务数据失败,formCode={}", formCode);
                return null;
            }

            BpmnStartConditionsVo? bpmnStartConditionsVo;
            try
            {
                var formAdaptor = formFactory.GetFormAdaptor(businessDataVo);
                bpmnStartConditionsVo = formAdaptor.PreviewSetCondition(businessDataVo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "条件评估失败:构造 BpmnStartConditionsVo 失败");
                return null;
            }

            // 8. 调用 IConditionService.CheckMatchCondition
            bool result = conditionService.CheckMatchCondition(nodeVo, conditionsConf, bpmnStartConditionsVo, false);
            _logger.LogInformation("条件评估结果:{}, nodeId={}, procInstId={}", result, nodeElementDto.NodeId, delegateTask.ProcInstId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "条件评估异常,procInstId={}, taskId={}", delegateTask.ProcInstId, delegateTask.Id);
            return null;
        }
    }
}