using antflowcore.constant.enums;
using AntFlowCore.Enums;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.service.biz;

public class BpmVariableMessageListenerService
{
    private readonly BpmVariableMessageService _bpmVariableMessageService;
    private readonly ProcessBusinessContansService _processBusinessContansService;

    public BpmVariableMessageListenerService(BpmVariableMessageService bpmVariableMessageService,
        ProcessBusinessContansService processBusinessContansService)
    {
        _bpmVariableMessageService = bpmVariableMessageService;
        _processBusinessContansService = processBusinessContansService;
    }
   /// <summary>
   /// check whether send by template
   /// </summary>
   /// <param name="bpmVariableMessageVo"></param>
   /// <returns></returns>
    public bool ListenerCheckIsSendByTemplate(BpmVariableMessageVo bpmVariableMessageVo) {
        return _bpmVariableMessageService.CheckIsSendByTemplate(bpmVariableMessageVo);
    }

   /// <summary>
   /// 监听发送模板消息
   /// </summary>
   /// <param name="bpmVariableMessageVo"></param>
    public void ListenerSendTemplateMessages(BpmVariableMessageVo bpmVariableMessageVo) {
        //build variable message
        BpmVariableMessageVo vo = _bpmVariableMessageService.GetBpmVariableMessageVo(bpmVariableMessageVo);
        //send template message
        if (vo!=null) {
            _bpmVariableMessageService.SendTemplateMessages(vo);
        }
    }
    public void SendProcessMessages(EventTypeEnum eventTypeEnum, BusinessDataVo vo){
        String processNumber=vo.ProcessNumber;
        String formCode = vo.FormCode;
        String startUserId=vo.StartUserId;
        bool isOutside = vo.IsOutSideAccessProc ?? false;
        BpmVariableMessageVo bpmVariableMessageVo = new BpmVariableMessageVo()
        {
            ProcessNumber = processNumber,
            FormCode = formCode,
            EventType = (int)eventTypeEnum,
            MessageType = eventTypeEnum.IsInNode() ? 2 : 1,
            EventTypeEnum = eventTypeEnum,
            Type = 1,
        };
        
        bool sendByTemplate = ListenerCheckIsSendByTemplate(bpmVariableMessageVo);
        if(sendByTemplate){
            bpmVariableMessageVo.IsOutside=isOutside;

            this.ListenerSendTemplateMessages(bpmVariableMessageVo);
        }else
        {
            ProcessInforVo processInforVo = new ProcessInforVo()
            {
                BusinessNumber = processNumber,
                FormCode = formCode,
                Type = 1
            };
            ActivitiBpmMsgVo msgVo = new ActivitiBpmMsgVo()
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