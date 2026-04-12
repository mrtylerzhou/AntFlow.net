

using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.util;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.bpmnprocessnotice;

public class EmailSendAdaptor: AbstractMessageSendAdaptor<MailInfo>
{
    public EmailSendAdaptor(IMessageService messageService,ILogger<EmailSendAdaptor> logger) : base(messageService,logger)
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