using antflowcore.adaptor.bpmnprocessnotice;
using antflowcore.constant.enums;
using AntFlowCore.Entity;
using antflowcore.service.biz;
using antflowcore.service.repository;
using antflowcore.vo;
using AntFlowCore.Vo;

namespace antflowcore.util;

using System;
using System.Collections.Generic;
using System.Linq;


public static class UserMsgUtils
{
    
    public static void SendMessages(UserMsgVo vo, params MessageSendTypeEnum[] types)
    {
        var service = GetMessageService();
        DoSendMessages(vo, service, types);
        InsertUserMessage(vo, service);
    }

    public static void SendMessagesNoUserMessage(UserMsgVo vo, params MessageSendTypeEnum[] types)
    {
        DoSendMessages(vo, GetMessageService(), types);
    }

    public static void InsertUserMessage(UserMsgVo vo)
    {
        InsertUserMessage(vo, GetMessageService());
    }

    private static void DoSendMessages(UserMsgVo vo, MessageService service, MessageSendTypeEnum[] types)
    {
        if (types == null || types.Length == 0 || !CheckEmployeeStatus(vo.UserId)) return;

        var typeList = types.ToList();

        if (typeList.Contains(MessageSendTypeEnum.MAIL))
            service.SendMail(BuildMailInfo(vo), vo.UserId);

        if (typeList.Contains(MessageSendTypeEnum.MESSAGE))
            service.SendSms(BuildMessageInfo(vo), vo.UserId);

        if (typeList.Contains(MessageSendTypeEnum.PUSH))
            service.SendAppPush(BuildBaseMsgInfo(vo), vo.UserId);
    }

    private static void InsertUserMessage(UserMsgVo vo, MessageService service)
    {
        if (!CheckEmployeeStatus(vo.UserId)) return;
        var msg = BuildUserMessage(vo);
        service.InsertUserMessage(msg);
    }

    public static void SendMessageBatch(List<UserMsgBatchVo> list)
    {
        var service = GetMessageService();
        DoSendMessageBatch(list, service);
        InsertUserMessageBatch(list, service);
    }

    public static void SendMessageBatchNoUserMessage(List<UserMsgBatchVo> list)
    {
        DoSendMessageBatch(list, GetMessageService());
    }

    public static void InsertUserMessageBatch(List<UserMsgBatchVo> list)
    {
        InsertUserMessageBatch(list, GetMessageService());
    }
    public static void SendMessageBathNoUserMessage(List<UserMsgBatchVo> userMsgBathVos) {

        MessageService messageService = GetMessageService();

        //执行发送信息(批量)
        DoSendMessageBatch(userMsgBathVos, messageService);

    }
    

    private static void DoSendMessageBatch(List<UserMsgBatchVo> list, MessageService service)
    {
        Dictionary<MessageSendTypeEnum, List<UserMsgVo>> map = FormatUserMsgBatchVos(list);
        foreach (KeyValuePair<MessageSendTypeEnum,List<UserMsgVo>> messageSendTypeEnumListEntry in map)
        {
            MessageSendTypeEnum messageSendTypeEnum = messageSendTypeEnumListEntry.Key;
            if (messageSendTypeEnum == null)
            {
                continue;
            }
            List<UserMsgVo> userMsgVos = messageSendTypeEnumListEntry.Value;
            IEnumerable<IProcessNoticeAdaptor> processNoticeAdaptors = ServiceProviderUtils.GetServices<IProcessNoticeAdaptor>();
            bool currentSend = false;
            foreach (IProcessNoticeAdaptor processNoticeAdaptor in processNoticeAdaptors)
            {
                if (processNoticeAdaptor!=null)
                {
                    if (processNoticeAdaptor.GetSupportCode() == messageSendTypeEnum.Code)
                    {
                        currentSend = true;
                        processNoticeAdaptor.SendMessageBatchByType(userMsgVos);
                    }
                }
            }

            if (!currentSend)
            {
              AfStaticLogUtil.Logger.LogInformation($"未实现的消息发送策略!{messageSendTypeEnum}");
            }
        }
    }

    private static void InsertUserMessageBatch(List<UserMsgBatchVo> list, MessageService service)
    {
        var messages = list
            .Where(x => CheckEmployeeStatus(x.UserMsgVo.UserId))
            .Select(x => BuildUserMessage(x.UserMsgVo))
            .ToList();

        service.InsertUserMessageBatch(messages);
    }

    private static Dictionary<MessageSendTypeEnum, List<UserMsgVo>> FormatUserMsgBatchVos(List<UserMsgBatchVo> list)
    {
        return list
            .Distinct()
            .Where(x => CheckEmployeeStatus(x.UserMsgVo.UserId) && x.MessageSendTypeEnums != null)
            .SelectMany(x => x.MessageSendTypeEnums.Select(type => new { Type = type, Vo = x.UserMsgVo }))
            .GroupBy(x => x.Type)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Vo).ToList());
    }

    private static UserMessage BuildUserMessage(UserMsgVo vo)
    {
        return new UserMessage
        {
            UserId = vo.UserId ?? "-999",
            Title = string.IsNullOrEmpty(vo.Title) ? vo.Content : vo.Title,
            Content = vo.Content,
            IsRead = false,
            Url = vo.Url,
            AppUrl = vo.AppPushUrl,
            Node = vo.TaskId,
            Source = vo.Source
        };
    }

    public static BaseMsgInfo BuildBaseMsgInfo(UserMsgVo vo)
    {
        return new BaseMsgInfo
        {
            MsgTitle = vo.Title,
            Content = vo.Content,
            Url = vo.AppPushUrl
        };
    }

    public static MessageInfo BuildMessageInfo(UserMsgVo vo)
    {
        return new MessageInfo
        {
            Receiver = vo.Mobile,
            Content = vo.Content
        };
    }

    public static MailInfo BuildMailInfo(UserMsgVo vo)
    {
        return new MailInfo
        {
            Receiver = vo.Email,
            Cc = vo.Cc,
            Title = vo.Title,
            Content = JoinEmailUrl(vo.SsoSessionDomain, vo.Content, vo.EmailUrl)
        };
    }

    private static string JoinEmailUrl(string domain, string content, string emailUrl)
    {
        if (!string.IsNullOrEmpty(emailUrl))
        {
            emailUrl = $"<a href='http://{domain}#{emailUrl}'>点击查看详情</a>";
        }
        return content + emailUrl;
    }

    private static bool CheckEmployeeStatus(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return false;
        var service = GetAfUserService();
        return service.CheckEmployeeEffective(userId) > 0;
    }

    private static UserService GetAfUserService()
    {
       return ServiceProviderUtils.GetService<UserService>();
    }

    private static MessageService GetMessageService()
    {
        return ServiceProviderUtils.GetService<MessageService>();
    }
}
