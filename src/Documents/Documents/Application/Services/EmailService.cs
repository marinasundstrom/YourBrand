using System.Net.Mail;

namespace YourBrand.Documents.Application.Services;

public class EmailService(ILogger<EmailService> logger) : IEmailService
{
    private readonly SmtpClient _smtpClient = new SmtpClient("localhost", 25);

    public async Task SendEmail(string recipient, string subject, string body)
    {
        var message = new MailMessage(new MailAddress("noreply@email.com", "Test sender"), new MailAddress(recipient));
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = true;
        message.BodyEncoding = System.Text.Encoding.UTF8;

        await _smtpClient.SendMailAsync(message);

        logger.LogInformation("Email was sent.");
    }
}