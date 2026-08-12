namespace Nms.Infrastructure.Notifications.Options;

public class NotificationOptions
{
    public const string SectionName = "Notifications";

    public SmtpOptions Smtp { get; set; } = new();
}

public class SmtpOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 25;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = "noreply@enterprise-nms.com";
    public bool EnableSsl { get; set; } = false;
}