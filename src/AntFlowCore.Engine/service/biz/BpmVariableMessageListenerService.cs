using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.util;

namespace AntFlowCore.Engine.service.biz;

/// <summary>
/// Variable message listener service.
/// Mirrors the Java <c>BpmVariableMessageListenerServiceImpl</c> / the entry point used by
/// <c>NextNodeProcessNoticeSendProcessor</c>: it decides whether to send notifications via
/// configured templates (read from JSON) or fall back to the default route-based message.
/// </summary>
public class BpmVariableMessageListenerService : IBpmVariableMessageListenerService
{
    private readonly IBpmVariableMessageBizService _bpmVariableMessageService;
    private readonly IProcessBusinessContansService _processBusinessContansService;

    public BpmVariableMessageListenerService(
        IBpmVariableMessageBizService bpmVariableMessageService,
        IProcessBusinessContansService processBusinessContansService)
    {
        _bpmVariableMessageService = bpmVariableMessageService;
        _processBusinessContansService = processBusinessContansService;
    }

    /// <summary>
    /// check whether send by template
    /// </summary>
    public bool ListenerCheckIsSendByTemplate(BpmVariableMessageVo bpmVariableMessageVo)
    {
        return _bpmVariableMessageService.CheckIsSendByTemplate(bpmVariableMessageVo);
    }

    /// <summary>
    /// 监听发送模板消息
    /// </summary>
    public void ListenerSendTemplateMessages(BpmVariableMessageVo bpmVariableMessageVo)
    {
        //build variable message
        BpmVariableMessageVo vo = _bpmVariableMessageService.GetBpmVariableMessageVo(bpmVariableMessageVo);
        //send template message
        if (vo != null)
        {
            _bpmVariableMessageService.SendTemplateMessages(vo);
        }
    }

    public void SendProcessMessages(EventTypeEnum eventTypeEnum, BusinessDataVo vo)
    {
        string processNumber = vo.ProcessNumber;
        string formCode = vo.FormCode;
        string startUserId = vo.StartUserId;
        bool isOutside = vo.IsOutSideAccessProc ?? false;
        BpmVariableMessageVo bpmVariableMessageVo = new BpmVariableMessageVo
        {
            ProcessNumber = processNumber,
            FormCode = formCode,
            EventType = (int)eventTypeEnum,
            MessageType = eventTypeEnum.IsInNode() ? 2 : 1,
            EventTypeEnum = eventTypeEnum,
            Type = 1,
        };

        bool sendByTemplate = ListenerCheckIsSendByTemplate(bpmVariableMessageVo);
        if (sendByTemplate)
        {
            bpmVariableMessageVo.IsOutside = isOutside;

            this.ListenerSendTemplateMessages(bpmVariableMessageVo);
        }
        else
        {
            ProcessInforVo processInforVo = new ProcessInforVo
            {
                BusinessNumber = processNumber,
                FormCode = formCode,
                Type = 1
            };
            ActivitiBpmMsgVo msgVo = new ActivitiBpmMsgVo
            {
                UserId = startUserId,
                ProcessId = processNumber,
                FormCode = formCode,
                ProcessType = "", //todo set process type
                EmailUrl = _processBusinessContansService.GetRoute(ProcessNoticeEnum.EMAIL_TYPE.Code, processInforVo,
                    isOutside),
                Url = _processBusinessContansService.GetRoute(ProcessNoticeEnum.EMAIL_TYPE.Code, processInforVo,
                    isOutside),
                AppPushUrl =
                    _processBusinessContansService.GetRoute(ProcessNoticeEnum.APP_TYPE.Code, processInforVo, isOutside),
                TaskId = null
            };
            ActivitiTemplateMsgUtils.sendBpmApprovalMsg(msgVo);
        }
    }
}
