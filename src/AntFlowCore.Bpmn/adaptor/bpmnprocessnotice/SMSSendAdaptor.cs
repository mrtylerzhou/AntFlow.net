

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