using AntFlow.Core.Entity;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Util;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Service.Business;

public class MessageService
{
    private readonly IBpmnEmployeeInfoProviderService _employeeInfoProvider;
    private readonly ILogger<MessageService> _logger;
    private readonly MailUtils _mailUtils;
    private readonly UserMessageService _userMessageService;
    private readonly UserMessageStatusService _userMessageStatusService;

    public MessageService(
        IBpmnEmployeeInfoProviderService employeeInfoProvider,
        UserMessageService userMessageService,
        UserMessageStatusService userMessageStatusService,
        MailUtils mailUtils,
        ILogger<MessageService> logger)
    {
        _employeeInfoProvider = employeeInfoProvider;
        _userMessageService = userMessageService;
        _userMessageStatusService = userMessageStatusService;
        _mailUtils = mailUtils;
        _logger = logger;
    }

    public void SendMail(MailInfo mailInfo, string userId)
    {
        UserMessageStatus? status = GetUserMessageStatus(userId);
        if (status == null || status.MailStatus == true)
        {
            _mailUtils.SendMailAsync(mailInfo);
        }
    }

    public void SendMailBatch(Dictionary<string, MailInfo> mailMap)
    {
        List<MailInfo>? mailsToSend = new();

        foreach (KeyValuePair<string, MailInfo> kvp in mailMap)
        {
            UserMessageStatus? status = GetUserMessageStatus(kvp.Key);
            if (status == null || status.MailStatus == true)
            {
                mailsToSend.Add(kvp.Value);
            }
        }

        if (mailsToSend.Any())
        {
            _mailUtils.SendMailBatchAsync(mailsToSend);
        }
    }

    public async Task SendSms(MessageInfo msgInfo, string userId)
    {
        UserMessageStatus? status = GetUserMessageStatus(userId);
        // TODO: 实现短信发送逻辑
    }

    public async Task SendSmsBatch(Dictionary<string, MessageInfo> smsMap)
    {
        List<MessageInfo>? list = new();

        foreach (KeyValuePair<string, MessageInfo> kvp in smsMap)
        {
            UserMessageStatus? status = GetUserMessageStatus(kvp.Key);
            // TODO: 实现批量短信发送逻辑
        }
    }

    public void SendAppPush(BaseMsgInfo msgInfo, string userId)
    {
        DoSendAppPush(msgInfo, userId);
    }

    public void SendAppPushBatch(Dictionary<string, BaseMsgInfo> map)
    {
        foreach (KeyValuePair<string, BaseMsgInfo> kvp in map)
        {
            DoSendAppPush(kvp.Value, kvp.Key);
        }
    }

    public void InsertUserMessage(UserMessage message)
    {
        if (string.IsNullOrWhiteSpace(message.UserId))
        {
            return;
        }

        DateTime now = DateTime.Now;
        string? user = SecurityUtils.GetLogInEmpNameSafe();

        message.CreateUser = user;
        message.CreateTime = now;
        message.UpdateUser = user;
        message.UpdateTime = now;

        _userMessageService.InsertMessage(message);
    }

    public void InsertUserMessageBatch(List<UserMessage> list)
    {
        DateTime now = DateTime.Now;
        string? user = SecurityUtils.GetLogInEmpNameSafe();

        foreach (UserMessage? msg in list)
        {
            if (string.IsNullOrWhiteSpace(msg.UserId))
            {
                continue;
            }

            msg.CreateUser = user;
            msg.CreateTime = now;
            msg.UpdateUser = user;
            msg.UpdateTime = now;
        }

        _userMessageService.SaveBatch(list);
    }

    private void DoSendAppPush(BaseMsgInfo msgInfo, string userId)
    {
        msgInfo.Username = GetUsernameByUserId(userId);
        UserMessageStatus? status = GetUserMessageStatus(userId);
        // TODO: 实现 App 推送逻辑
    }

    private UserMessageStatus GetUserMessageStatus(string userId)
    {
        return _userMessageStatusService.baseRepo.Where(x => x.UserId == userId).ToOne();
    }

    private string GetUsernameByUserId(string userId)
    {
        Dictionary<string, string>? dict = _employeeInfoProvider.ProvideEmployeeInfo(new List<string> { userId });
        return dict.TryGetValue(userId, out string? name) ? name : string.Empty;
    }
}