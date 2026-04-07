using AntFlowCore.Enums;
using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IBpmVariableMessageListenerService
{
    bool ListenerCheckIsSendByTemplate(BpmVariableMessageVo bpmVariableMessageVo);
    void ListenerSendTemplateMessages(BpmVariableMessageVo bpmVariableMessageVo);
    void SendProcessMessages(EventTypeEnum eventTypeEnum, BusinessDataVo vo);
}
