

using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Bpmn;
using AntFlowCore.Bpmn.adaptor.bpmnprocessnotice;
using AntFlowCore.Bpmn.util;
using AntFlowCore.Common.constant.enums;
using AntFlowCore.Common.util;
using AntFlowCore.Core;
using AntFlowCore.Engine.Engine.service.biz;
using AntFlowCore.Vo;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Extensions.Extensions.adaptor.bpmnprocessnotice;

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