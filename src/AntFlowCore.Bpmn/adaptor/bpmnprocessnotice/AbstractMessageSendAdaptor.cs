using AntFlowCore.Abstraction.adaptor;
using AntFlowCore.Abstraction.service.biz;
using AntFlowCore.Base.extension;
using AntFlowCore.Base.vo;
using Microsoft.Extensions.Logging;

namespace AntFlowCore.Bpmn.adaptor.bpmnprocessnotice;

public abstract class AbstractMessageSendAdaptor<T>:  IProcessNoticeAdaptor
{
    protected readonly IMessageService _messageService;
    private readonly ILogger _logger;

    public AbstractMessageSendAdaptor(IMessageService messageService, ILogger logger)
    {
        _messageService = messageService;
        _logger = logger;
    }

    protected Dictionary<string, T> MessageProcessing(List<UserMsgVo> userMsgVos, Func<UserMsgVo, T> fun)
    {
        if (userMsgVos.IsEmpty())
        {
            _logger.LogInformation("发送的消息内容不能为空!");
            return null;
        }

        Dictionary<string, T> dic = new Dictionary<string, T>();
        foreach (UserMsgVo userMsgVo in userMsgVos)
        {
            T result = fun(userMsgVo);
            dic[userMsgVo.UserId] = result;
        }

        return dic;
    }
    public abstract void SendMessageBatchByType(List<UserMsgVo> userMsgVos);
    public abstract int GetSupportCode();

}