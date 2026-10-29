using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.ChatApp.Features.Organizations;
using YourBrand.ChatApp.Features.Users;
using YourBrand.IdentityManagement.Contracts;

using YourBrand.ChatApp.Features.Chat;
using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.Specifications;
using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.ChatApp.Infrastructure.Persistence;
using YourBrand.Domain;

namespace YourBrand.ChatApp.Consumers;

public record PostMessage2(string Id, string OrganizationId, string ChannelId, string? ReplyToId, string Content);

public class MessageProcessorConsumer(IMediator mediator, IUserContext userContext, YourBrand.ChatApp.Features.Chat.IChatNotificationService chatNotificationService,
        ApplicationDbContext applicationDbContext, ILogger<MessageProcessorConsumer> logger) : IConsumer<PostMessage2>
{
    public async Task Consume(ConsumeContext<PostMessage2> context)
    {
        var request = context.Message;

        var channel = await applicationDbContext.Channels
            .InOrganization(request.OrganizationId)
            .FirstOrDefaultAsync(x => x.Id == context.Message.ChannelId, context.CancellationToken);

        if (channel is null)
        {
            return;
        }

        if (context.Message.Content.ToLower().Contains("test")) 
        {
            logger.LogInformation("Rejected message");
            await chatNotificationService.NotifyMessageSendError(request.ChannelId, userContext.UserId, request.Id, "Forbidden content");
            return;
        }

        await PostMessage3(context.Message, channel, context.CancellationToken);
    }

    private async Task<string> PostMessage3(PostMessage2 request, Channel channel, CancellationToken cancellationToken)
    {
        Message message;

        if (request.ReplyToId is not null)
        {
            message = new Message(new MessageId(request.Id), request.OrganizationId, request.ChannelId, request.ReplyToId, request.Content);
        }
        else
        {
            message = new Message(new MessageId(request.Id), request.OrganizationId, request.ChannelId, request.Content);
        }

        message.PostedBy = await applicationDbContext.ChannelParticipants
            .FirstOrDefaultAsync(x => x.UserId == userContext.UserId, cancellationToken);

        applicationDbContext.Messages.Add(message);

        var participant = channel.Participants.First(x => x.UserId == userContext.UserId);

        message.Posted = DateTimeOffset.UtcNow;
        message.PostedById = participant.Id;

        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return message.Id;
    }
}