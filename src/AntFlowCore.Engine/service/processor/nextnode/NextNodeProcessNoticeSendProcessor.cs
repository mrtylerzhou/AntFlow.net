using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Abstraction.service.repository;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.dto;
using AntFlowCore.Base.entity;
using AntFlowCore.Base.entity.jsonconf;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.util;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.service.processor;
using AntFlowCore.Bpmn.util;
using AntFlowCore.Core.vo;
using AntFlowCore.Persist.api.interf.repository;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Engine.service.processor.nextnode;

/// <summary>
/// 下一节点消息通知处理器:发送审批通知(模板消息或默认渠道消息).
/// 对应 Java NextNodeProcessNoticeSendProcessor. Order=2,在标签处理(order=0)和委托(order=1)之后执行.
/// </summary>
public class NextNodeProcessNoticeSendProcessor : INextNodeTaskProcessor
{
    private readonly IBpmVariableMessageListenerService _bpmVariableMessageListenerService;
    private readonly IProcessBusinessContansService _processBusinessContansService;
    private readonly IBpmBusinessProcessService _bpmBusinessProcessService;
    private readonly IBpmnConfService _bpmnConfService;
    private readonly ILogger<NextNodeProcessNoticeSendProcessor> _logger;

    public NextNodeProcessNoticeSendProcessor(
        IBpmVariableMessageListenerService bpmVariableMessageListenerService,
        IProcessBusinessContansService processBusinessContansService,
        IBpmBusinessProcessService bpmBusinessProcessService,
        IBpmnConfService bpmnConfService,
        ILogger<NextNodeProcessNoticeSendProcessor> logger)
    {
        _bpmVariableMessageListenerService = bpmVariableMessageListenerService;
        _processBusinessContansService = processBusinessContansService;
        _bpmBusinessProcessService = bpmBusinessProcessService;
        _bpmnConfService = bpmnConfService;
        _logger = logger;
    }

    public int Order() => 2;

    public void PostProcess(BpmNextTaskDto dto)
    {
        BpmAfTask delegateTask = dto.DelegateTask;
        string processNumber = dto.ProcessNumber;

        BpmBusinessProcess? bpmBusinessProcess = _bpmBusinessProcessService._repository
            .Find(a => a.BusinessNumber == processNumber)
            .FirstOrDefault();
        if (bpmBusinessProcess == null)
        {
            _logger.LogError("消息通知发送失败:流程实例不存在,流程号={}", processNumber);
            return;
        }

        BpmnConf? bpmnConf = _bpmnConfService._repository
            .Find(a => a.BpmnCode == bpmBusinessProcess.Version)
            .FirstOrDefault();
        if (bpmnConf == null)
        {
            _logger.LogError("消息通知发送失败:流程配置不存在,流程号={}", processNumber);
            return;
        }

        string formCode = bpmBusinessProcess.ProcessinessKey;
        bool isOutside = (bpmnConf.IsOutSideProcess ?? 0) == 1;
        string bpmnCode = bpmnConf.BpmnCode;

        BpmVariableMessageVo bpmVariableMessageVo = new BpmVariableMessageVo
        {
            ProcessNumber = processNumber,
            FormCode = formCode,
            EventType = (int)EventTypeEnum.PROCESS_FLOW,
            MessageType = EventTypeEnum.PROCESS_FLOW.IsInNode() ? 2 : 1,
            ElementId = delegateTask.TaskDefKey,
            Assignee = delegateTask.Assignee,
            EventTypeEnum = EventTypeEnum.PROCESS_FLOW,
            Type = 2,
        };

        string? bpmnConfConfConfigJson = bpmnConf.ConfConfigJson;
        if (string.IsNullOrEmpty(bpmnConfConfConfigJson))
        {
            return;
        }
        BpmnConfConfigJson? bpmnConfConfigJson = JsonConfUtil.ParseConfConfig(bpmnConfConfConfigJson);
        List<int>? noticeChannelTypes = bpmnConfConfigJson?.NoticeChannelTypes;
        if (noticeChannelTypes.IsEmpty())
        {
            return;
        }

        bool sendByTemplate = _bpmVariableMessageListenerService.ListenerCheckIsSendByTemplate(bpmVariableMessageVo);
        if (sendByTemplate)
        {
            bpmVariableMessageVo.IsOutside = isOutside;
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
            string emailUrl = _processBusinessContansService.GetRoute(ProcessNoticeEnum.EMAIL_TYPE.Code, processInforVo, isOutside);
            string appUrl = _processBusinessContansService.GetRoute(ProcessNoticeEnum.APP_TYPE.Code, processInforVo, isOutside);
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
}
