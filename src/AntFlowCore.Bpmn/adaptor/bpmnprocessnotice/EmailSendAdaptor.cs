

using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Bpmn;
using AntFlowCore.Bpmn.adaptor.bpmnprocessnotice;
using AntFlowCore.Bpmn.util;
using AntFlowCore.Common.util;
using AntFlowCore.Core;
using AntFlowCore.Core.constant.enums;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Extensions.Extensions.adaptor.bpmnprocessnotice;

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