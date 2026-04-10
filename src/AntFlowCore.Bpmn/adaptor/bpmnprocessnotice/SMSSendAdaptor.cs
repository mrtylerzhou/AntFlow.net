using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.util;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.bpmnprocessnotice;

public class SMSSendAdaptor: AbstractMessageSendAdaptor<MessageInfo>
{
    public SMSSendAdaptor(IMessageService messageService, ILogger<SMSSendAdaptor> logger) : base(messageService, logger)
    {
    }

    public override void SendMessageBatchByType(List<UserMsgVo> userMsgVos)
    {
        Dictionary<String, MessageInfo> stringMailInfoMap = base.MessageProcessing(userMsgVos, UserMsgUtils.BuildMessageInfo);
        _messageService.SendSmsBatch(stringMailInfoMap);
    }

    public override int GetSupportCode()
    {
        return ProcessNoticeEnum.PHONE_TYPE.Code;
    }
}