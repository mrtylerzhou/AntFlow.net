namespace antflowcore.dto;

/// <summary>
/// 邮件服务配置项
/// </summary>
public class MailSettings
{
    /// <summary>
    /// SMTP服务器地址，例如：smtp.163.com
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// SMTP端口号，常用端口：587（TLS）、465（SSL）
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 邮箱账号（发送方）
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// 邮箱密码或授权码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 是否启用SSL/TLS
    /// </summary>
    public bool EnableSsl { get; set; } = true;
}