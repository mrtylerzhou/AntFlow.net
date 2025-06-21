using AntFlowCore.Entities;
using AntFlowCore.Entity;
using antflowcore.service.repository;
using antflowcore.util;
using AntFlowCore.Vo;

namespace antflowcore.service.biz;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class MessageService
{
    private readonly IBpmnEmployeeInfoProviderService _employeeInfoProvider;
    private readonly UserMessageService _userMessageService;
    private readonly UserMessageStatusService _userMessageStatusService;
    private readonly MailUtils _mailUtils;
    private readonly ILogger<MessageService> _logger;

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

    public void  SendMail(MailInfo mailInfo, string userId)
    {
        var status =  GetUserMessageStatus(userId);
        if (status == null || status.MailStatus == true)
        {
             _mailUtils.SendMailAsync(mailInfo);
        }
    }

    public void SendMailBatch(Dictionary<string, MailInfo> mailMap)
    {
        var mailsToSend = new List<MailInfo>();

        foreach (var kvp in mailMap)
        {
            var status =  GetUserMessageStatus(kvp.Key);
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
        var status = GetUserMessageStatus(userId);
        // TODO: 根据短信服务实现短信发送逻辑
    }

    public async Task SendSmsBatch(Dictionary<string, MessageInfo> smsMap)
    {
        var list = new List<MessageInfo>();

        foreach (var kvp in smsMap)
        {
            var status = GetUserMessageStatus(kvp.Key);
            // TODO: 根据短信服务实现短信发送逻辑
        }
    }

    public void SendAppPush(BaseMsgInfo msgInfo, string userId)
    {
         DoSendAppPush(msgInfo, userId);
    }

    public void SendAppPushBatch(Dictionary<string, BaseMsgInfo> map)
    {
        foreach (var kvp in map)
        {
             DoSendAppPush(kvp.Value, kvp.Key);
        }
    }

    public void InsertUserMessage(UserMessage message)
    {
        if (string.IsNullOrWhiteSpace(message.UserId))
            return;

        var now = DateTime.Now;
        var user = SecurityUtils.GetLogInEmpNameSafe();

        message.CreateUser = user;
        message.CreateTime = now;
        message.UpdateUser = user;
        message.UpdateTime = now;

         _userMessageService.InsertMessage(message);
    }

    public void InsertUserMessageBatch(List<UserMessage> list)
    {
        var now = DateTime.Now;
        var user = SecurityUtils.GetLogInEmpNameSafe();

        foreach (var msg in list)
        {
            if (string.IsNullOrWhiteSpace(msg.UserId)) continue;
            msg.CreateUser = user;
            msg.CreateTime = now;
            msg.UpdateUser = user;
            msg.UpdateTime = now;
        }

        _userMessageService.SaveBatch(list);
    }

    private void DoSendAppPush(BaseMsgInfo msgInfo, string userId)
    {
        msgInfo.Username =  GetUsernameByUserId(userId);
        var status =  GetUserMessageStatus(userId);
        // TODO: 根据 App 推送服务实现
    }

    private UserMessageStatus GetUserMessageStatus(string userId)
    {
        return _userMessageStatusService.baseRepo.Where(x => x.UserId == userId).ToOne();
    }

    private string GetUsernameByUserId(string userId)
    {
        var dict =  _employeeInfoProvider.ProvideEmployeeInfo(new List<string> { userId });
        return dict.TryGetValue(userId, out var name) ? name : string.Empty;
    }
}
