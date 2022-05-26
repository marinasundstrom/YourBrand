using YourBrand.Documents.Application.Services;
using YourBrand.Documents.Contracts;

using MassTransit;

namespace YourBrand.Documents.Consumers;

public class SendEmailConsumer : IConsumer<SendEmail>
{
    private readonly IEmailService _emailService;

    public SendEmailConsumer(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<SendEmail> context)
    {
        var message = context.Message;

        await _emailService.SendEmail(message.Recipient, message.Subject, message.Body);
    }
}