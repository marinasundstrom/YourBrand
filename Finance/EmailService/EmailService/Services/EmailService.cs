using System.Net.Mail;

namespace Worker.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly SmtpClient _smtpClient;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;

        // Fake server
        _smtpClient = new SmtpClient("localhost", 25);
    }

    public async Task SendEmail(string recipient, string subject, string body)
    {
        var message = new MailMessage(new MailAddress("noreply@email.com", "Test sender"), new MailAddress(recipient));
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = true;
        message.BodyEncoding = System.Text.Encoding.UTF8;

        await _smtpClient.SendMailAsync(message);

        _logger.LogInformation("Email was sent.");
    }
}