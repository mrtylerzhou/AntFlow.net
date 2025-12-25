using System.Net.Mail;
using antflowcore.dto;
using antflowcore.util.Extension;
using AntFlowCore.Vo;
using MailKit.Security;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace antflowcore.util;

using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


public class MailUtils
{
    private readonly MailSettings _settings;
    private readonly ILogger<MailUtils> _logger;

    private MailUtils()
    {
        
    }
    public MailUtils(IOptionsMonitor<MailSettings> options, ILogger<MailUtils> logger)
    {
        _settings = options.CurrentValue;
        _logger = logger;
    }

    public async Task SendMailBatchAsync(
        IEnumerable<MailInfo> mails,
        CancellationToken ct = default)
    {
        if (mails == null) return;

        var mailList = mails
            .Where(m => m != null)
            .ToList();

        if (!mailList.Any()) return;

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.SslOnConnect,
                ct);

            await client.AuthenticateAsync(
                _settings.Account,
                _settings.Password,
                ct);

            foreach (var mail in mailList)
            {
                try
                {
                    var message = CreateMimeMessage(mail);
                    await client.SendAsync(message, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "发送邮件失败，Title={Title}",
                        mail.Title);
                }
            }
        }
        finally
        {
            await client.DisconnectAsync(true, ct);
        }
    }

    public async Task SendMailAsync(MailInfo mail, CancellationToken ct = default)
    {
        var message = CreateMimeMessage(mail);

        using var client = new SmtpClient();
        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
        try
        {
            
            await client.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.SslOnConnect,
                ct);

            await client.AuthenticateAsync(
                _settings.Account,
                _settings.Password,
                ct);

            await client.SendAsync(message, ct);
        }
        finally
        {
            await client.DisconnectAsync(true, ct);
        }
    }

    private MimeMessage CreateMimeMessage(MailInfo mail)
    {
        var message = new MimeMessage();

        // 发件人
        message.From.Add(
            MailboxAddress.Parse(_settings.Account));

        // 收件人（单个）
        if (!string.IsNullOrWhiteSpace(mail.Receiver))
        {
            message.To.Add(MailboxAddress.Parse(mail.Receiver));
        }

        // 收件人（多个）
        if (mail.Receivers != null)
        {
            foreach (var r in mail.Receivers.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                message.To.Add(MailboxAddress.Parse(r));
            }
        }

        // 抄送
        if (!mail.Cc.IsEmpty())
        {
            foreach (var cc in mail.Cc)
            {
                message.Cc.Add(MailboxAddress.Parse(cc));
            }
        }

        message.Subject = TruncateTitle(mail.Title);

        var body = new BodyBuilder
        {
            HtmlBody = mail.Content ?? string.Empty
        };

        // 文件附件（磁盘）
        if (mail.File != null && File.Exists(mail.File.FullName))
        {
            body.Attachments.Add(mail.File.FullName);
        }

        // 流附件
        if (mail.FileInputStream != null)
        {
            body.Attachments.Add(
                mail.FileName ?? "attachment",
                mail.FileInputStream);
        }

        message.Body = body.ToMessageBody();

        return message;
    }
    
    

    private string TruncateTitle(string title)
    {
        const int titleLimit = 130;
        if (string.IsNullOrWhiteSpace(title)) return string.Empty;
        return title.Length > titleLimit ? title.Substring(0, titleLimit) + "..." : title;
    }
}
