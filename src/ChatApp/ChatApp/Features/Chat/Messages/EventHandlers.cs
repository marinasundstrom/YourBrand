using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

using YourBrand.ChatApp.Common;
using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.ChatApp.Features.Chat.Messages.EventHandlers;

public sealed class MessagePostedEventHandler(
    ISettableTenantContext tenantContext,
    IMessageRepository messagesRepository,
    IUserRepository userRepository,
    IChatNotificationService chatNotificationService,
    IDtoComposer dtoComposer) : IDomainEventHandler<MessagePosted>
{
    public async Task Handle(MessagePosted notification, CancellationToken cancellationToken)
    {
        tenantContext.SetTenantId(notification.TenantId);

        var message = await messagesRepository.FindByIdAsync(notification.MessageId, cancellationToken);

        if (message is null)
            return;

        await SendConfirmationToSender(message, cancellationToken);

        var user = await userRepository.FindByIdAsync(message.PostedBy!.UserId, cancellationToken);

        if (user is null)
            return;

        await NotifyChannel(message, user, cancellationToken);
    }

    private async Task SendConfirmationToSender(Message message, CancellationToken cancellationToken)
    {
        await chatNotificationService.SendConfirmationToSender(
            message.ChannelId, message.PostedBy!.UserId, message.Id, cancellationToken);
    }

    private async Task NotifyChannel(Message message, User user, CancellationToken cancellationToken)
    {
        MessageDto messageDto = await dtoComposer.ComposeMessageDto(message, cancellationToken);

        await chatNotificationService.NotifyMessagePosted(
            messageDto, cancellationToken);
    }
}

public sealed class MessageEditedEventHandler(
    ISettableTenantContext tenantContext,
    IMessageRepository messagesRepository,
    IUserRepository userRepository,
    IChatNotificationService chatNotificationService) : IDomainEventHandler<MessageEdited>
{
    public async Task Handle(MessageEdited notification, CancellationToken cancellationToken)
    {
        tenantContext.SetTenantId(notification.TenantId);

        var message = await messagesRepository.FindByIdAsync(notification.MessageId);
        var user = await userRepository.FindByIdAsync(message!.LastEditedBy!.UserId);

        await chatNotificationService.NotifyMessageEdited(
            notification.ChannelId, new MessageEditedData(notification.MessageId, message.LastEdited.GetValueOrDefault(), new ParticipantData(message!.LastEditedBy!.Id.ToString(), message!.LastEditedBy.DisplayName, message!.LastEditedBy.UserId), notification.Content), cancellationToken);
    }
}

public sealed class MessageDeletedEventHandler(
    ISettableTenantContext tenantContext,
    IChannelRepository channelRepository,
    IMessageRepository messagesRepository,
    IUserRepository userRepository,
    IChatNotificationService chatNotificationService) : IDomainEventHandler<MessageDeleted>
{
    public async Task Handle(MessageDeleted notification, CancellationToken cancellationToken)
    {
        tenantContext.SetTenantId(notification.TenantId);

        var channel = await channelRepository.FindByIdAsync(notification.ChannelId, cancellationToken);

        var shouldSoftDelete = channel.Settings.SoftDeleteMessages.GetValueOrDefault();

        if (shouldSoftDelete)
        {
            var message = await messagesRepository.FindByIdAsync(notification.MessageId);
            var user = await userRepository.FindByIdAsync(message!.DeletedBy!.UserId);

            await chatNotificationService.NotifyMessageDeleted(
                notification.ChannelId, new MessageDeletedData(notification.MessageId, false, message!.Deleted.GetValueOrDefault(), new ParticipantData(message.DeletedById.ToString()!, user.Name, user.Id)), cancellationToken);
        }
        else
        {
            await chatNotificationService.NotifyMessageDeleted(
                notification.ChannelId, new MessageDeletedData(notification.MessageId, true, null, null), cancellationToken);
        }
    }
}

public sealed class UserReactedToMessageEventHandler(
    ISettableTenantContext tenantContext,
    IChannelRepository channelRepository,
    IMessageRepository messagesRepository,
    IUserRepository userRepository,
    IDtoFactory dtoFactory,
    IChatNotificationService chatNotificationService) : IDomainEventHandler<UserReactedToMessage>
{
    public async Task Handle(UserReactedToMessage notification, CancellationToken cancellationToken)
    {
        tenantContext.SetTenantId(notification.TenantId);

        var message = await messagesRepository.FindByIdAsync(notification.MessageId, cancellationToken);

        var reaction = message!.Reactions.Last();

        var channel = await channelRepository.FindByIdAsync(message.ChannelId, cancellationToken);

        var participant = channel.Participants.FirstOrDefault(x => x.Id == notification.ParticipantId);

        var user = await userRepository.FindByIdAsync(participant!.UserId, cancellationToken);

        var reactionDto = dtoFactory.CreateReactionDto(reaction, participant, new Dictionary<Domain.ValueObjects.ChannelParticipantId, User> { { participant.Id, user! } });

        await chatNotificationService.NotifyReaction(
            notification.ChannelId, message.Id, reactionDto, cancellationToken);
    }
}

public sealed class UserRemovedReactionFromMessageEventHandler(
    ISettableTenantContext tenantContext,
    IChatNotificationService chatNotificationService) : IDomainEventHandler<UserRemovedReactionFromMessage>
{
    public async Task Handle(UserRemovedReactionFromMessage notification, CancellationToken cancellationToken)
    {
        tenantContext.SetTenantId(notification.TenantId);

        var reaction2 = notification.Reaction;

        await chatNotificationService.NotifyReactionRemoved(
            notification.ChannelId, notification.MessageId, reaction2, notification.ParticipantId.ToString()!, cancellationToken);
    }
}