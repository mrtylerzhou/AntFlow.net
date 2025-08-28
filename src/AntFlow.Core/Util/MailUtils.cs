using AntFlow.Core.Dto;
using AntFlow.Core.Util.Extension;
using AntFlow.Core.Vo;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace AntFlow.Core.Util;

public class MailUtils
{
    private readonly ILogger<MailUtils> _logger;
    private readonly MailSettings _settings;

    public MailUtils(IOptions<MailSettings> options, ILogger<MailUtils> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task SendMailAsync(MailInfo mail)
    {
        try
        {
            using SmtpClient? smtpClient = CreateSmtpClient();
            using MailMessage? message = CreateMailMessage(mail);
            await smtpClient.SendMailAsync(message);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "发送邮件失败");
        }
    }

    public async Task SendMailBatchAsync(List<MailInfo> mails)
    {
        try
        {
            using SmtpClient? smtpClient = CreateSmtpClient();
            foreach (MailInfo? mail in mails)
            {
                using MailMessage? message = CreateMailMessage(mail);
                await smtpClient.SendMailAsync(message);
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "批量发送邮件失败");
        }
    }

    private SmtpClient CreateSmtpClient()
    {
        return new SmtpClient(_settings.Host)
        {
            Port = _settings.Port,
            Credentials = new NetworkCredential(_settings.Account, _settings.Password),
            EnableSsl = _settings.EnableSsl
        };
    }

    private MailMessage CreateMailMessage(MailInfo mail)
    {
        MailMessage? message = new()
        {
            From = new MailAddress(_settings.Account),
            Subject = TruncateTitle(mail.Title),
            Body = mail.Content ?? string.Empty,
            IsBodyHtml = true
        };

        if (!string.IsNullOrWhiteSpace(mail.Receiver))
        {
            message.To.Add(mail.Receiver);
        }

        if (mail.Receivers != null && mail.Receivers.Any())
        {
            foreach (string? r in mail.Receivers)
            {
                if (!string.IsNullOrWhiteSpace(r))
                {
                    message.To.Add(r);
                }
            }
        }

        if (!mail.Cc.IsEmpty())
        {
            foreach (string cc in mail.Cc)
            {
                message.CC.Add(cc);
            }
        }

        if (mail.File != null && File.Exists(mail.File.FullName))
        {
            message.Attachments.Add(new Attachment(mail.File.FullName));
        }

        if (mail.FileInputStream != null)
        {
            Attachment? attachment = new(mail.FileInputStream, mail.FileName ?? "attachment");
            message.Attachments.Add(attachment);
        }

        return message;
    }

    private string TruncateTitle(string title)
    {
        const int titleLimit = 130;
        if (string.IsNullOrWhiteSpace(title))
        {
            return string.Empty;
        }

        return title.Length > titleLimit ? title.Substring(0, titleLimit) + "..." : title;
    }
}