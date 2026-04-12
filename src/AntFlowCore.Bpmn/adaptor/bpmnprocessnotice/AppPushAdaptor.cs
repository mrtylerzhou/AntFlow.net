using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.constant.enums;
using AntFlowCore.Base.vo;
using AntFlowCore.Bpmn.util;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.bpmnprocessnotice;

public class AppPushAdaptor: AbstractMessageSendAdaptor<BaseMsgInfo>
{
    public AppPushAdaptor(IMessageService messageService, ILogger<AppPushAdaptor> logger) : base(messageService, logger)
    {
    }

    public override void SendMessageBatchByType(List<UserMsgVo> userMsgVos)
    {
        Dictionary<String, BaseMsgInfo> stringBaseMsgInfoMap = base.MessageProcessing(userMsgVos, UserMsgUtils.BuildBaseMsgInfo);
        _messageService.SendAppPushBatch(stringBaseMsgInfoMap);
    }

    public override int GetSupportCode()
    {
        return ProcessNoticeEnum.APP_TYPE.Code;
    }
}