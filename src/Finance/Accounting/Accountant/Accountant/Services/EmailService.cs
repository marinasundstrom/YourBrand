namespace YourBrand.Accountant.Services;

using MassTransit;

using YourBrand.Notifications.Contracts;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public EmailService(ILogger<EmailService> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task SendEmail(string recipient, string subject, string body)
    {
        await _publishEndpoint.Publish(new SendEmail(recipient, subject, body));

        _logger.LogInformation("Email was sent.");
    }
}