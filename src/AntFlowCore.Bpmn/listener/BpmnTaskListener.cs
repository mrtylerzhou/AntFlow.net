using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.adaptor.formoperation;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
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

        // 自动节点: 评估条件并自动完成任务
        if (ProcessAutomaticNode(delegateTask, bpmBusinessProcess, bpmnConf, formCode, isOutside))
        {
            return; // task was auto-completed, skip notifications
        }

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
    /// 自动节点处理:检查任务的FormKey中是否包含automaticNode标签,
    /// 如果是自动节点,则评估条件并自动完成任务,记录审批信息.
    /// 对应 Java NextNodeLabelsProcessor.processAutomaticNode.
    /// </summary>
    /// <returns>true 如果任务是自动节点并已被自动完成; false otherwise</returns>
    private bool ProcessAutomaticNode(BpmAfTask delegateTask, BpmBusinessProcess bpmBusinessProcess,
        BpmnConf bpmnConf, string formCode, bool isOutside)
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

        bool isAutomaticNode = false;
        foreach (var nodeLabelVO in extraInfoDTO.NodeLabelVOS)
        {
            if (StringConstants.AUTOMATIC_NODE.Equals(nodeLabelVO.LabelValue))
            {
                isAutomaticNode = true;
                break;
            }
        }

        if (!isAutomaticNode)
        {
            return false;
        }

        // 构建业务数据VO用于条件评估
        string processNumber = delegateTask.ProcessNumber;
        string elementId = delegateTask.TaskDefKey;
        int? isLowCodeFlow = bpmnConf.IsLowCodeFlow;

        BusinessDataVo businessDataVo;
        if (isLowCodeFlow == 1)
        {
            businessDataVo = new UDLFApplyVo();
        }
        else
        {
            businessDataVo = new BusinessDataVo();
        }
        businessDataVo.ProcessNumber = processNumber;
        businessDataVo.TaskDefKey = elementId;
        businessDataVo.FormCode = formCode;
        businessDataVo.IsLowCodeFlow = isLowCodeFlow;
        businessDataVo.IsOutSideAccessProc = isOutside;

        // 获取表单适配器
        IFormFactory formFactory = ServiceProviderUtils.GetService<IFormFactory>();
        IFormOperationAdaptor<BusinessDataVo>? formAdaptor = formFactory?.GetFormAdaptor(businessDataVo);
        if (formAdaptor == null)
        {
            _logger.LogError("未能根据流程formcode找到流程适配器信息! formCode={FormCode}", formCode);
            return false;
        }

        // 加载表单数据(低代码流程需要lfFields进行条件评估)
        if (isLowCodeFlow == 1 && !string.IsNullOrEmpty(bpmBusinessProcess.BusinessId))
        {
            businessDataVo.BusinessId = bpmBusinessProcess.BusinessId;
            try
            {
                formAdaptor.OnQueryData(businessDataVo);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "自动节点加载表单数据失败, processNumber={ProcessNumber}, elementId={ElementId}", processNumber, elementId);
            }

            // 如果lfConditions为空,则用lfFields填充(对应Java逻辑)
            if (businessDataVo is UDLFApplyVo udlfVo)
            {
                if ((udlfVo.LfConditions == null || udlfVo.LfConditions.Count == 0) && udlfVo.LfFields != null)
                {
                    udlfVo.LfConditions = udlfVo.LfFields;
                }
            }
        }

        string assigneeName = AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Desc;
        bool? conditionResult = null;
        try
        {
            conditionResult = formAdaptor.AutomaticCondition(businessDataVo);
            formAdaptor.AutomaticAction(businessDataVo, conditionResult);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "自动节点条件判断或动作执行异常, processNumber={ProcessNumber}, elementId={ElementId}", processNumber, elementId);
        }
        finally
        {
            // 自动完成任务
            var taskService = ServiceProviderUtils.GetService<ITaskService>();
            taskService?.Complete(delegateTask);

            // 记录审批信息
            BpmVerifyInfo bpmVerifyInfo = new BpmVerifyInfo
            {
                VerifyDate = DateTime.Now,
                TaskName = delegateTask.Name,
                TaskId = delegateTask.Id,
                RunInfoId = bpmBusinessProcess.ProcInstId,
                VerifyUserId = AFSpecialAssigneeEnum.AUTO_NODE_SKIP.Id,
                VerifyUserName = assigneeName,
                TaskDefKey = delegateTask.TaskDefKey,
                VerifyStatus = (int)ProcessSubmitStateEnum.PROCESS_AGRESS_TYPE,
                VerifyDesc = string.Format(StringConstants.AF_AUTO_EVALUATE_SKIP_COMMENT, conditionResult),
                ProcessCode = processNumber,
                TenantId = MultiTenantUtil.GetCurrentTenantId(),
            };
            _bpmVerifyInfoService.AddVerifyInfo(bpmVerifyInfo);
        }

        return true;
    }
}