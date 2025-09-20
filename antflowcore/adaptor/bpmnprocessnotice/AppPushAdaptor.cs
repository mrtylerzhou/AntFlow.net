using antflowcore.constant.enums;
using antflowcore.service.biz;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.adaptor.bpmnprocessnotice;

public class AppPushAdaptor: AbstractMessageSendAdaptor<BaseMsgInfo>
{
    public AppPushAdaptor(MessageService messageService, ILogger<AppPushAdaptor> logger) : base(messageService, logger)
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