# AntFlowCore .NET 通知扩展指南

## 1. 概述

通知扩展允许自定义审批通知的发送渠道。AntFlowCore 内置了邮件、短信、App推送三种通知方式，同时预留了企业微信、钉钉、飞书等扩展接口。当需要添加自定义通知渠道（如内部IM、Webhook等）时，可以通过实现 `IProcessNoticeAdaptor` 接口来扩展。

## 2. 通知系统架构

```
审批节点完成
     │
     ▼
┌──────────────────────────────────────────┐
│  NextNodeProcessNoticeSendProcessor      │
│  (通知发送处理器)                         │
│  - 解析流程配置获取通知渠道列表           │
│  - 构建消息VO                            │
└──────────────────┬───────────────────────┘
                   │
                   ▼
┌──────────────────────────────────────────┐
│  IProcessNoticeAdaptor                   │
│  - SendMessageBatchByType()              │
│  - GetSupportCode()                      │
└──────────────────┬───────────────────────┘
                   │
     ┌─────────────┼─────────────┐
     │             │             │
     ▼             ▼             ▼
┌────────┐  ┌──────────┐  ┌────────┐
│Email   │  │AppPush   │  │SMS     │
│Send    │  │Adaptor   │  │Send    │
│Adaptor │  │          │  │Adaptor │
└────────┘  └──────────┘  └────────┘
```

## 3. 通知类型枚举

```csharp
public class ProcessNoticeEnum
{
    public static readonly ProcessNoticeEnum EMAIL_TYPE     = new(1, "邮件");
    public static readonly ProcessNoticeEnum PHONE_TYPE     = new(2, "短信");
    public static readonly ProcessNoticeEnum APP_TYPE       = new(3, "app推送");
    public static readonly ProcessNoticeEnum SYSTEM_TYPE    = new(4, "系统消息");
    public static readonly ProcessNoticeEnum WECHAT_TYPE    = new(5, "企微");
    public static readonly ProcessNoticeEnum DING_TALK_TYPE = new(6, "钉钉");
    public static readonly ProcessNoticeEnum FEISHU_TYPE    = new(7, "飞书");
}
```

| 编码 | 渠道 | 内置支持 |
|------|------|---------|
| 1 | 邮件 | ✅ |
| 2 | 短信 | ✅ |
| 3 | App推送 | ✅ |
| 4 | 系统消息 | ✅ |
| 5 | 企业微信 | ❌（需扩展） |
| 6 | 钉钉 | ❌（需扩展） |
| 7 | 飞书 | ❌（需扩展） |

## 4. 通知适配器接口

### 4.1 IProcessNoticeAdaptor

```csharp
public interface IProcessNoticeAdaptor
{
    /// <summary>
    /// 批量发送消息
    /// </summary>
    void SendMessageBatchByType(List<UserMsgVo> userMsgVos);
    
    /// <summary>
    /// 获取支持的通知类型编码
    /// </summary>
    int GetSupportCode();
}
```

### 4.2 AbstractMessageSendAdaptor

抽象基类，提供消息预处理能力：

```csharp
public abstract class AbstractMessageSendAdaptor<T> : IProcessNoticeAdaptor
{
    protected readonly IMessageService _messageService;
    private readonly ILogger _logger;

    protected AbstractMessageSendAdaptor(IMessageService messageService, ILogger logger)
    {
        _messageService = messageService;
        _logger = logger;
    }

    /// <summary>
    /// 消息预处理：将UserMsgVo列表转换为Dictionary
    /// </summary>
    protected Dictionary<string, T> MessageProcessing(
        List<UserMsgVo> userMsgVos, Func<UserMsgVo, T> fun)
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
```

### 4.3 IMessageService

消息服务接口，定义各渠道的发送方法：

```csharp
public interface IMessageService
{
    void SendMail(MailInfo mailInfo, string userId);
    void SendMailBatch(Dictionary<string, MailInfo> mailMap);
    Task SendSms(MessageInfo msgInfo, string userId);
    Task SendSmsBatch(Dictionary<string, MessageInfo> smsMap);
    void SendAppPush(BaseMsgInfo msgInfo, string userId);
    void SendAppPushBatch(Dictionary<string, BaseMsgInfo> map);
    void InsertUserMessage(UserMessage message);
    void InsertUserMessageBatch(List<UserMessage> list);
}
```

## 5. 内置通知适配器实现

### 5.1 EmailSendAdaptor（邮件通知）

```csharp
public class EmailSendAdaptor : AbstractMessageSendAdaptor<MailInfo>
{
    public EmailSendAdaptor(
        IMessageService messageService, 
        ILogger<EmailSendAdaptor> logger) : base(messageService, logger)
    {
    }

    public override void SendMessageBatchByType(List<UserMsgVo> userMsgVos)
    {
        Dictionary<string, MailInfo> mailMap = base.MessageProcessing(
            userMsgVos, UserMsgUtils.BuildMailInfo);
        _messageService.SendMailBatch(mailMap);
    }

    public override int GetSupportCode()
    {
        return ProcessNoticeEnum.EMAIL_TYPE.Code;  // 返回 1
    }
}
```

### 5.2 AppPushAdaptor（App推送通知）

```csharp
public class AppPushAdaptor : AbstractMessageSendAdaptor<BaseMsgInfo>
{
    public AppPushAdaptor(
        IMessageService messageService, 
        ILogger<AppPushAdaptor> logger) : base(messageService, logger)
    {
    }

    public override void SendMessageBatchByType(List<UserMsgVo> userMsgVos)
    {
        Dictionary<string, BaseMsgInfo> msgMap = base.MessageProcessing(
            userMsgVos, UserMsgUtils.BuildBaseMsgInfo);
        _messageService.SendAppPushBatch(msgMap);
    }

    public override int GetSupportCode()
    {
        return ProcessNoticeEnum.APP_TYPE.Code;  // 返回 3
    }
}
```

### 5.3 SMSSendAdaptor（短信通知）

```csharp
public class SMSSendAdaptor : AbstractMessageSendAdaptor<MessageInfo>
{
    public SMSSendAdaptor(
        IMessageService messageService, 
        ILogger<SMSSendAdaptor> logger) : base(messageService, logger)
    {
    }

    public override void SendMessageBatchByType(List<UserMsgVo> userMsgVos)
    {
        Dictionary<string, MessageInfo> smsMap = base.MessageProcessing(
            userMsgVos, UserMsgUtils.BuildMessageInfo);
        _messageService.SendSmsBatch(smsMap);
    }

    public override int GetSupportCode()
    {
        return ProcessNoticeEnum.PHONE_TYPE.Code;  // 返回 2
    }
}
```

## 6. 自定义通知适配器步骤

### Step 1: 定义通知类型

```csharp
public class ProcessNoticeEnum
{
    // ... 现有类型
    public static readonly ProcessNoticeEnum WEBHOOK_TYPE = new(10, "Webhook回调");
    public static readonly ProcessNoticeEnum INTERNAL_IM_TYPE = new(11, "内部IM");
}
```

### Step 2: 创建消息信息类

```csharp
/// <summary>
/// Webhook消息信息
/// </summary>
public class WebhookMsgInfo
{
    public string WebhookUrl { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public Dictionary<string, object> Payload { get; set; }
    public Dictionary<string, string> Headers { get; set; }
}
```

### Step 3: 扩展 IMessageService

```csharp
public interface IMessageService
{
    // ... 现有方法
    Task SendWebhookBatch(Dictionary<string, WebhookMsgInfo> webhookMap);
    Task SendInternalImBatch(Dictionary<string, InternalImMsgInfo> imMap);
}
```

### Step 4: 实现通知适配器

```csharp
/// <summary>
/// Webhook回调通知适配器
/// </summary>
public class WebhookSendAdaptor : AbstractMessageSendAdaptor<WebhookMsgInfo>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WebhookSendAdaptor(
        IMessageService messageService, 
        IHttpClientFactory httpClientFactory,
        ILogger<WebhookSendAdaptor> logger) : base(messageService, logger)
    {
        _httpClientFactory = httpClientFactory;
    }

    public override void SendMessageBatchByType(List<UserMsgVo> userMsgVos)
    {
        // 1. 预处理消息
        Dictionary<string, WebhookMsgInfo> webhookMap = base.MessageProcessing(
            userMsgVos, BuildWebhookInfo);
        
        // 2. 批量发送
        if (webhookMap != null && webhookMap.Any())
        {
            _messageService.SendWebhookBatch(webhookMap);
        }
    }

    public override int GetSupportCode()
    {
        return ProcessNoticeEnum.WEBHOOK_TYPE.Code;  // 返回 10
    }

    /// <summary>
    /// 构建Webhook消息信息
    /// </summary>
    private WebhookMsgInfo BuildWebhookInfo(UserMsgVo userMsgVo)
    {
        return new WebhookMsgInfo
        {
            WebhookUrl = GetWebhookUrl(userMsgVo.UserId),
            Title = userMsgVo.Title,
            Content = userMsgVo.Content,
            Payload = new Dictionary<string, object>
            {
                ["userId"] = userMsgVo.UserId,
                ["processNumber"] = userMsgVo.ProcessNumber,
                ["taskId"] = userMsgVo.TaskId,
                ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            },
            Headers = new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["X-AntFlow-Signature"] = GenerateSignature(userMsgVo)
            }
        };
    }

    private string GetWebhookUrl(string userId)
    {
        // 从用户配置或全局配置获取Webhook URL
        return $"https://your-company.com/webhook/approval/{userId}";
    }

    private string GenerateSignature(UserMsgVo vo)
    {
        // 生成请求签名
        return Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes($"{vo.UserId}{vo.TaskId}SECRET_KEY")));
    }
}
```

### Step 5: 企业微信通知适配器示例

```csharp
/// <summary>
/// 企业微信通知适配器
/// </summary>
public class WechatWorkSendAdaptor : AbstractMessageSendAdaptor<WechatWorkMsgInfo>
{
    private readonly IWechatWorkApiClient _wechatApi;

    public WechatWorkSendAdaptor(
        IMessageService messageService,
        IWechatWorkApiClient wechatApi,
        ILogger<WechatWorkSendAdaptor> logger) : base(messageService, logger)
    {
        _wechatApi = wechatApi;
    }

    public override void SendMessageBatchByType(List<UserMsgVo> userMsgVos)
    {
        foreach (var userMsgVo in userMsgVos)
        {
            try
            {
                var msgInfo = new WechatWorkMsgInfo
                {
                    ToUser = userMsgVo.UserId,
                    AgentId = "YOUR_AGENT_ID",
                    Title = userMsgVo.Title,
                    Description = userMsgVo.Content,
                    Url = userMsgVo.Url,
                    BtnTxt = "查看详情"
                };
                _wechatApi.SendMessage(msgInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "企业微信通知发送失败: {UserId}", userMsgVo.UserId);
            }
        }
    }

    public override int GetSupportCode()
    {
        return ProcessNoticeEnum.WECHAT_TYPE.Code;  // 返回 5
    }
}

public class WechatWorkMsgInfo
{
    public string ToUser { get; set; }
    public string AgentId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public string BtnTxt { get; set; }
}
```

### Step 6: 注册到 DI 容器

```csharp
// 在 ServiceRegistration.AntFlowServiceSetUp 中
services.AddSingleton<IProcessNoticeAdaptor, WebhookSendAdaptor>();
services.AddSingleton<IProcessNoticeAdaptor, WechatWorkSendAdaptor>();
```

## 7. 通知模板配置

### 7.1 在流程配置中设置通知渠道

在流程模板的 `conf_config_json` 中配置通知渠道列表：

```json
{
    "noticeChannelTypes": [1, 3, 10],
    "deduplicationType": 1
}
```

其中 `noticeChannelTypes` 是通知类型编码数组，引擎会根据列表依次调用对应的通知适配器。

### 7.2 通知模板表（information_template）

| 字段 | 说明 |
|------|------|
| `system_title` | 系统消息标题 |
| `system_content` | 系统消息内容 |
| `mail_title` | 邮件标题 |
| `mail_content` | 邮件内容（支持变量） |
| `note_content` | 短信内容 |
| `jump_url` | 跳转URL类型（1=审批页，2=详情页，3=待办列表） |
| `evt` | 事件类型（提交/同意/拒绝/退回等） |

### 7.3 模板变量

通知模板支持以下变量：

| 变量 | 说明 |
|------|------|
| `{processName}` | 流程名称 |
| `{submitUser}` | 发起人姓名 |
| `{submitTime}` | 发起时间 |
| `{taskName}` | 任务名称 |
| `{nodeName}` | 节点名称 |
| `{verifyUser}` | 审批人姓名 |
| `{verifyTime}` | 审批时间 |
| `{verifyDesc}` | 审批意见 |

## 8. 通知发送流程

```
审批节点完成
     │
     ▼
┌──────────────────────────────────────────┐
│  NextNodeProcessNoticeSendProcessor      │
│  1. 查询流程配置中的通知渠道列表          │
│  2. 构建 UserMsgVo 列表                  │
│  3. 保存系统消息到 user_message 表       │
└──────────────────┬───────────────────────┘
                   │
                   ▼
┌──────────────────────────────────────────┐
│  遍历通知渠道列表                         │
│  对每个渠道：                             │
│  1. 查找匹配的 IProcessNoticeAdaptor     │
│  2. 调用 SendMessageBatchByType()        │
└──────────────────┬───────────────────────┘
                   │
         ┌─────────┴─────────┐
         │                   │
         ▼                   ▼
    ┌────────┐          ┌────────┐
    │ 渠道1  │          │ 渠道2  │
    │适配器  │          │适配器  │
    └────────┘          └────────┘
```

## 9. 通知消息存储

系统消息在发送前会先持久化到 `user_message` 表：

```csharp
public class UserMessage
{
    public long Id { get; set; }
    public string UserId { get; set; }       // 接收人ID
    public string Title { get; set; }        // 消息标题
    public string Content { get; set; }      // 消息内容
    public string Url { get; set; }          // 跳转URL
    public string Node { get; set; }         // 节点ID
    public bool IsRead { get; set; }         // 是否已读
    public bool IsDel { get; set; }          // 是否删除
    public string TenantId { get; set; }     // 租户ID
    public DateTime? CreateTime { get; set; }
    public string AppUrl { get; set; }       // App端URL
    public int Source { get; set; }          // 消息来源
}
```

## 10. 最佳实践

### 10.1 异步发送

通知发送应尽量使用异步方式，避免阻塞主流程：

```csharp
public override async void SendMessageBatchByType(List<UserMsgVo> userMsgVos)
{
    var msgMap = base.MessageProcessing(userMsgVos, BuildWebhookInfo);
    if (msgMap != null)
    {
        await _messageService.SendWebhookBatch(msgMap);
    }
}
```

### 10.2 失败重试

```csharp
public override void SendMessageBatchByType(List<UserMsgVo> userMsgVos)
{
    foreach (var userMsgVo in userMsgVos)
    {
        int retryCount = 0;
        const int maxRetries = 3;
        
        while (retryCount < maxRetries)
        {
            try
            {
                var msgInfo = BuildWebhookInfo(userMsgVo);
                _messageService.SendWebhook(msgInfo, userMsgVo.UserId);
                break; // 发送成功，退出重试
            }
            catch (Exception ex)
            {
                retryCount++;
                _logger.LogWarning(ex, 
                    "Webhook发送失败 (尝试 {Retry}/{Max}): {UserId}", 
                    retryCount, maxRetries, userMsgVo.UserId);
                
                if (retryCount >= maxRetries)
                {
                    _logger.LogError("Webhook最终发送失败: {UserId}", userMsgVo.UserId);
                }
                else
                {
                    Thread.Sleep(1000 * retryCount); // 指数退避
                }
            }
        }
    }
}
```

### 10.3 消息幂等

确保同一通知不会重复发送：

```csharp
// 在消息处理前检查是否已发送
public override void SendMessageBatchByType(List<UserMsgVo> userMsgVos)
{
    var filteredList = userMsgVo
        .Where(msg => !IsAlreadySent(msg.UserId, msg.TaskId))
        .ToList();
    
    if (filteredList.Any())
    {
        var msgMap = base.MessageProcessing(filteredList, BuildWebhookInfo);
        _messageService.SendWebhookBatch(msgMap);
    }
}
```

## 11. 常见问题

| 问题 | 解决方案 |
|------|---------|
| 通知未发送 | 检查流程配置 `noticeChannelTypes` 是否包含该渠道编码 |
| 适配器未生效 | 确认 `GetSupportCode()` 返回值与配置编码一致 |
| 消息内容为空 | 检查通知模板配置和变量替换逻辑 |
| 发送超时 | 使用异步发送并设置合理的超时时间 |
| 模板不生效 | 确认 `InformationTemplate` 中事件类型与实际匹配 |
