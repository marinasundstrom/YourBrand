namespace YourBrand.Accountant.Services;

using MassTransit;

using YourBrand.Notifications.Contracts;

public class EmailService(ILogger<EmailService> logger, IPublishEndpoint publishEndpoint) : IEmailService
{
    public async Task SendEmail(string recipient, string subject, string body)
    {
        await publishEndpoint.Publish(new SendEmail(recipient, subject, body));

        logger.LogInformation("Email was sent.");
    }
}