using AntFlow.Core.Constant.Enums;
using AntFlow.Core.Entity;
using AntFlow.Core.Service.Business;
using AntFlow.Core.Service.Repository;
using AntFlow.Core.Vo;

namespace AntFlow.Core.Util;

public static class UserMsgUtils
{
    public static void SendMessages(UserMsgVo vo, params MessageSendTypeEnum[] types)
    {
        MessageService? service = GetMessageService();
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
        if (types == null || types.Length == 0 || !CheckEmployeeStatus(vo.UserId))
        {
            return;
        }

        List<MessageSendTypeEnum>? typeList = types.ToList();

        if (typeList.Contains(MessageSendTypeEnum.MAIL))
        {
            service.SendMail(BuildMailInfo(vo), vo.UserId);
        }

        if (typeList.Contains(MessageSendTypeEnum.MESSAGE))
        {
            service.SendSms(BuildMessageInfo(vo), vo.UserId);
        }

        if (typeList.Contains(MessageSendTypeEnum.PUSH))
        {
            service.SendAppPush(BuildBaseMsgInfo(vo), vo.UserId);
        }
    }

    private static void InsertUserMessage(UserMsgVo vo, MessageService service)
    {
        if (!CheckEmployeeStatus(vo.UserId))
        {
            return;
        }

        UserMessage? msg = BuildUserMessage(vo);
        service.InsertUserMessage(msg);
    }

    public static void SendMessageBatch(List<UserMsgBatchVo> list)
    {
        MessageService? service = GetMessageService();
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

    public static void SendMessageBathNoUserMessage(List<UserMsgBatchVo> userMsgBathVos)
    {
        MessageService messageService = GetMessageService();

        //执行发送消息(批量)
        DoSendMessageBatch(userMsgBathVos, messageService);
    }


    private static void DoSendMessageBatch(List<UserMsgBatchVo> list, MessageService service)
    {
        Dictionary<MessageSendTypeEnum, List<UserMsgVo>>? map = FormatUserMsgBathVos(list);

        if (map.TryGetValue(MessageSendTypeEnum.MAIL, out List<UserMsgVo>? mailList))
        {
            Dictionary<string, MailInfo>? dict = mailList.ToDictionary(v => v.UserId, BuildMailInfo);
            service.SendMailBatch(dict);
        }

        if (map.TryGetValue(MessageSendTypeEnum.MESSAGE, out List<UserMsgVo>? msgList))
        {
            Dictionary<string, MessageInfo>? dict = msgList.ToDictionary(v => v.UserId, BuildMessageInfo);
            service.SendSmsBatch(dict);
        }

        if (map.TryGetValue(MessageSendTypeEnum.PUSH, out List<UserMsgVo>? pushList))
        {
            Dictionary<string, BaseMsgInfo>? dict = pushList.ToDictionary(v => v.UserId, BuildBaseMsgInfo);
            service.SendAppPushBatch(dict);
        }
    }

    private static void InsertUserMessageBatch(List<UserMsgBatchVo> list, MessageService service)
    {
        List<UserMessage>? messages = list
            .Where(x => CheckEmployeeStatus(x.UserMsgVo.UserId))
            .Select(x => BuildUserMessage(x.UserMsgVo))
            .ToList();

        service.InsertUserMessageBatch(messages);
    }

    private static Dictionary<MessageSendTypeEnum, List<UserMsgVo>> FormatUserMsgBathVos(List<UserMsgBatchVo> list)
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

    private static BaseMsgInfo BuildBaseMsgInfo(UserMsgVo vo)
    {
        return new BaseMsgInfo { MsgTitle = vo.Title, Content = vo.Content, Url = vo.AppPushUrl };
    }

    private static MessageInfo BuildMessageInfo(UserMsgVo vo)
    {
        return new MessageInfo { Receiver = vo.Mobile, Content = vo.Content };
    }

    private static MailInfo BuildMailInfo(UserMsgVo vo)
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
        if (string.IsNullOrEmpty(userId))
        {
            return false;
        }

        UserService? service = GetAfUserService();
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