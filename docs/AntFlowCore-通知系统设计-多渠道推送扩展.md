# AntFlowCore 通知系统设计：邮件/短信/App 推送扩展

## 前言

流程审批中，通知非常重要：新任务来了要通知审批人，审批完成要通知发起人。AntFlowCore 设计了非常灵活的通知系统，支持多种通知渠道，你也可以轻松扩展自定义渠道（比如钉钉、企业微信）。本文详细介绍通知系统设计和扩展方法。

## 整体设计思路

AntFlowCore 通知系统设计非常简洁：

- **抽象接口**：`IProcessNoticeAdaptor` 定义统一通知接口
- **多个实现**：每个渠道一个实现（邮件、短信、App 推送）
- **配置化**：可以配置不同场景使用不同渠道
- **开闭原则**：新增渠道不需要修改核心代码，只需要新增实现

核心接口 `IProcessNoticeAdaptor`:

```csharp
public interface IProcessNoticeAdaptor
{
    /// <summary>
    /// 发送通知
    /// </summary>
    /// <param name="targetUserIds">目标用户ID列表</param>
    /// <param name="title">通知标题</param>
    /// <param name="content">通知内容</param>
    /// <returns>是否发送成功</returns>
    Task<bool> SendAsync(List<string> targetUserIds, string title, string content);
}
```

非常简单，就是发送通知给目标用户。

## 内置支持的通知渠道

AntFlowCore 已经内置了这几种渠道：

| 渠道 | 实现类 | 说明 |
|------|--------|------|
| 邮件 | `EmailSendAdaptor` | SMTP 发送邮件 |
| 短信 | `SMSSendAdaptor` | 短信渠道，需要对接第三方短信服务商 |
| App 推送 | `AppPushAdaptor` | 推送给前端App，通过 websocket 推送 |

我们逐个来看。

## 邮件通知：EmailSendAdaptor

### 配置

在 `appsettings.json` 配置邮件：

```json
{
  "MailSettings": {
    "Server": "smtp.qq.com",
    "Port": 587,
    "Username": "your@email.com",
    "Password": "your-password",
    "EnableSsl": true,
    "From": "your@email.com",
    "FromName": "AntFlow 审批通知"
  }
}
```

### 源码

```csharp
public class EmailSendAdaptor : IProcessNoticeAdaptor
{
    private readonly IOptions<MailSettings> _mailSettings;
    private readonly ILogger<EmailSendAdaptor> _logger;

    public EmailSendAdaptor(
        IOptions<MailSettings> mailSettings,
        ILogger<EmailSendAdaptor> logger)
    {
        _mailSettings = mailSettings;
        _logger = logger;
    }

    public async Task<bool> SendAsync(
        List<string> targetUserIds, 
        string title, 
        string content)
    {
        var settings = _mailSettings.Value;
        var users = await _userService.GetUsersByIds(targetUserIds);
        var toAddresses = users.Select(u => new MailAddress(u.Email, u.Name)).ToList();

        using var mail = new MailMessage();
        mail.From = new MailAddress(settings.From, settings.FromName);
        mail.Subject = title;
        mail.Body = content;
        mail.IsBodyHtml = true;
        foreach (var to in toAddresses)
        {
            mail.To.Add(to);
        }

        using var client = new SmtpClient();
        await client.ConnectAsync(settings.Server, settings.Port, settings.EnableSsl);
        await client.AuthenticateAsync(settings.Username, settings.Password);
        await client.SendAsync(mail);
        await client.DisconnectAsync(true);

        _logger.LogInformation($"邮件通知发送成功，{targetUserIds.Count} 个收件人");
        return true;
    }
}
```

就是标准的 SMTP 发送邮件，配置简单，直接能用。

## App 推送：AppPushAdaptor

如果你有前端 App，可以用 websocket 推送通知给在线用户：

```csharp
public class AppPushAdaptor : IProcessNoticeAdaptor
{
    private readonly IWebSocketManager _webSocketManager;
    private readonly IUserService _userService;

    public async Task<bool> SendAsync(
        List<string> targetUserIds, 
        string title, 
        string content)
    {
        foreach (var userId in targetUserIds)
        {
            // 给每个用户推送通知
            await _webSocketManager.PublishToUser(userId, new 
            {
                Type = "approval_notification",
                Title = title,
                Content = content,
                CreateTime = DateTime.Now
            });
        }
        return true;
    }
}
```

在线用户立即收到弹窗通知，体验更好。

## 什么时候发送通知

AntFlowCore 在这些场景自动发送通知：

| 场景 | 通知谁 |
|------|--------|
| 新任务创建 | 通知审批人 |
| 任务完成 | 通知发起人 |
| 流程驳回 | 通知发起人 |
| 流程结束 | 通知发起人 |
| 被加签 | 通知新加的审批人 |

核心代码 `BpmnVariableMessageListener.cs`:

```csharp
public class BpmnVariableMessageListener : IFlowEventHandler<ProcessStartedEvent>
{
    private readonly IEnumerable<IProcessNoticeAdaptor> _adaptors;
    private readonly MessageService _messageService;

    public async Task Handle(ProcessStartedEvent @event)
    {
        // 获取所有需要通知的审批人
        var assignees = GetTaskAssignees(@event.Task);
        
        // 获取流程名称
        var processName = @event.ProcessInstance.ProcessName;
        
        // 用所有启用的渠道发送通知
        foreach (var adaptor in _adaptors.Where(a => a.Enabled))
        {
            await adaptor.SendAsync(
                assignees, 
                $"你有新的审批任务: {processName}",
                $"请及时处理");
        }
        
        // 保存通知记录到数据库
        await _messageService.SaveNotification(assignees, processName);
    }
}
```

通过事件系统触发通知，核心和业务完全解耦。

## 配置多渠道同时发送

你可以配置同时启用多个渠道，比如：

```csharp
// 注册所有你需要的渠道
services.AddSingleton<IProcessNoticeAdaptor, EmailSendAdaptor>();
services.AddSingleton<IProcessNoticeAdaptor, AppPushAdaptor>();
// 添加短信
// services.AddSingleton<IProcessNoticeAdaptor, SMSSendAdaptor>();
```

系统会遍历所有启用的渠道，每个渠道都发送一次。所以审批人会同时收到邮件 + App 推送，不会漏掉。

## 扩展自定义渠道：钉钉/企业微信

如果你需要钉钉或者企业微信通知，扩展非常简单，只需要三步：

### 第一步：实现接口

```csharp
public class DingTalkSendAdaptor : IProcessNoticeAdaptor
{
    private readonly IDingTalkClient _dingTalkClient;
    private readonly ILogger<DingTalkSendAdaptor> _logger;

    public DingTalkSendAdaptor(
        IDingTalkClient dingTalkClient,
        ILogger<DingTalkSendAdaptor> logger)
    {
        _dingTalkClient = dingTalkClient;
        _logger = logger;
    }

    public bool Enabled => true; // 可以根据配置决定是否启用

    public async Task<bool> SendAsync(
        List<string> targetUserIds, 
        string title, 
        string content)
    {
        // 1. 根据用户ID获取钉钉 userid
        var dingTalkUserIds = await _userService.ToDingTalkUserIds(targetUserIds);
        
        // 2. 调用钉钉 API 发送工作通知
        var result = await _dingTalkClient.SendWorkNotification(
            dingTalkUserIds, 
            title, 
            content);
            
        _logger.LogInformation("钉钉通知发送完成");
        return result.IsSuccess;
    }
}
```

### 第二步：注册到 DI

```csharp
services.AddSingleton<IProcessNoticeAdaptor, DingTalkSendAdaptor>();
```

### 第三步：完成

不需要修改任何其他代码，就能用了！就是这么简单。

## 通知记录

AntFlowCore 会把所有通知记录保存到数据库 `af_user_message` 表，方便查询：

```csharp
public async Task SaveNotification(
    List<string> userIds, 
    string content)
{
    foreach (var userId in userIds)
    {
        await _userMessageRepo.InsertAsync(new UserMessage
        {
            UserId = userId,
            Title = title,
            Content = content,
            IsRead = false,
            CreateTime = DateTime.Now,
            TenantId = MultiTenantUtil.GetCurrentTenantId()
        });
    }
}
```

用户可以在系统中查看历史通知，方便追溯。

## 整体架构图

```
┌─────────────────────────────────────────────────────────────┐
│                    事件触发                               │
│  (任务创建 / 流程完成 / 驳回)                           │
└────────────────┬────────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────────┐
│              获取所有注册的通知渠道                         │
└────────────────┬────────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────────┐
│        遍历每个渠道，调用 SendAsync                       │
│         - EmailSendAdaptor                              │
│         - AppPushAdaptor                                │
│         - DingTalkSendAdaptor  ← 你自定义              │
└────────────────┬────────────────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────────────────┐
│                  保存通知记录                               │
└─────────────────────────────────────────────────────────────┘
```

## 最佳实践

### 1. 多渠道冗余发送

重要审批通知建议同时开邮件 + 钉钉/App推送，保证审批人能收到，不会漏掉。

### 2. 配置开关

可以根据环境配置是否启用某个渠道：

```csharp
public bool Enabled => _configuration.GetValue<bool>("DingTalk:Enabled");
```

开发环境可以只开邮件，生产环境开所有渠道。

### 3. 失败处理

如果某个渠道发送失败（比如短信服务商宕机），AntFlowCore 会：
- 记录日志告诉你发送失败
- 其他渠道继续发送，不会因为一个渠道失败影响其他渠道
- 通知记录还是会保存，用户可以在系统里看到

## 常见问题

### 1. 可以只开一种渠道吗？

可以，你只注册你需要的渠道就行，没注册的不会被调用。

### 2. 可以根据不同流程用不同渠道吗？

可以，你可以在 `SendAsync` 里面根据流程信息判断选择渠道，非常灵活。

### 3. 需要给不同用户不同渠道吗？

可以，你实现 `SendAsync` 的时候，根据用户偏好选择渠道，比如用户偏好钉钉就用钉钉，偏好邮件就用邮件。

### 4. 支持短信吗？

支持，已经预留了 `SMSSendAdaptor` 接口，你只需要对接你的短信服务商 API 就行了，框架已经做好了所有集成工作。

## 总结

AntFlowCore 通知系统设计非常简洁优雅：

- **接口抽象**：所有渠道都实现同一个接口，核心代码不需要关心具体实现
- **开闭原则**：新增渠道只需要加新实现，不需要改核心代码
- **多渠道同时发送**：支持多个渠道同时发送，保证通知到位
- **扩展简单**：三步就能添加自定义渠道，非常方便

不管你需要什么通知渠道，都可以轻松接入。

---

**相关链接：**
- [AntFlowCore 事件系统详解](./AntFlowCore-事件系统详解-监听流程状态变化.md)
- [AntFlowCore 平行网关实现原理详解](./AntFlowCore-平行网关实现原理详解.md)
- [上一篇：平行网关实现原理详解](./AntFlowCore-平行网关实现原理详解.md)
