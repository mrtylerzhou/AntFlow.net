using AntFlowCore.Core.entity;
using AntFlowCore.Vo;

namespace AntFlowCore.Abstraction.service.biz;

public interface IMessageService
{
    void SendMail(MailInfo mailInfo, string userId);
    void SendMailBatch(Dictionary<string, MailInfo> mailMap);
    Task SendSms(MessageInfo msgInfo, string userId);
    Task SendSmsBatch(Dictionary<string, MessageInfo> smsMap);
    void SendAppPush(BaseMsgInfo msgInfo, string userId);
    void SendAppPushBatch(Dictionary<string, BaseMsgInfo> map);
    void InsertUserMessage(UserMessage message);
    void InsertUserMessageBatch(List<UserMessage> list);
}
