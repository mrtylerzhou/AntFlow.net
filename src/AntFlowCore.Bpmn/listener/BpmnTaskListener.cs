using AntFlowCore.Abstraction.Orm.util;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.exception;
using AntFlowCore.Base.extension;
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
}