namespace Backend.Application.Notifications;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
}

public class SendEmailRequestInput
{
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string HtmlMessage { get; set; } = string.Empty;
}