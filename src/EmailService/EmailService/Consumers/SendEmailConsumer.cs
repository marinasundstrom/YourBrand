using MassTransit;

using YourBrand.Notifications.Contracts;
using YourBrand.Notifications.Services;

namespace YourBrand.Notifications.Consumers;

public class SendEmailConsumer(IEmailService emailService) : IConsumer<SendEmail>
{
    public async Task Consume(ConsumeContext<SendEmail> context)
    {
        var message = context.Message;

        await emailService.SendEmail(message.Recipient, message.Subject, message.Body);
    }
}