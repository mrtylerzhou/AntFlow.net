using antflowcore.constant.enums;
using antflowcore.service.biz;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.bpmnprocessnotice;

public class EmailSendAdaptor: AbstractMessageSendAdaptor<MailInfo>
{
    public EmailSendAdaptor(MessageService messageService,ILogger<EmailSendAdaptor> logger) : base(messageService,logger)
    {
    }
    

    public override void SendMessageBatchByType(List<UserMsgVo> userMsgVos)
    {
       
        Dictionary<String, MailInfo> stringMailInfoMap = base.MessageProcessing(userMsgVos, UserMsgUtils.BuildMailInfo);
        _messageService.SendMailBatch(stringMailInfoMap);
    }

    public override int GetSupportCode()
    {
        return ProcessNoticeEnum.EMAIL_TYPE.Code;
    }
}