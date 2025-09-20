using antflowcore.constant.enums;
using antflowcore.service.biz;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.bpmnprocessnotice;

public class SMSSendAdaptor: AbstractMessageSendAdaptor<MessageInfo>
{
    public SMSSendAdaptor(MessageService messageService, ILogger<SMSSendAdaptor> logger) : base(messageService, logger)
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