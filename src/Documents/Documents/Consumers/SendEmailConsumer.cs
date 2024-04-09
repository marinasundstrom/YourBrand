using MassTransit;

using YourBrand.Documents.Application.Services;
using YourBrand.Documents.Contracts;

namespace YourBrand.Documents.Consumers;

public class SendEmailConsumer(IEmailService emailService) : IConsumer<SendEmail>
{
    public async Task Consume(ConsumeContext<SendEmail> context)
    {
        var message = context.Message;

        await emailService.SendEmail(message.Recipient, message.Subject, message.Body);
    }
}