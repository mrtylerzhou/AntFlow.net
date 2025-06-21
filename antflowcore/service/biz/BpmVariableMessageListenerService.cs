using antflowcore.service.repository;
using AntFlowCore.Vo;

namespace antflowcore.service.biz;

public class BpmVariableMessageListenerService
{
    private readonly BpmVariableMessageService _bpmVariableMessageService;

    public BpmVariableMessageListenerService(BpmVariableMessageService bpmVariableMessageService)
    {
        _bpmVariableMessageService = bpmVariableMessageService;
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
    public void listenerSendTemplateMessages(BpmVariableMessageVo bpmVariableMessageVo) {
        //build variable message
        BpmVariableMessageVo vo = _bpmVariableMessageService.GetBpmVariableMessageVo(bpmVariableMessageVo);
        //send template message
        if (vo!=null) {
            _bpmVariableMessageService.SendTemplateMessages(vo);
        }
    }
}