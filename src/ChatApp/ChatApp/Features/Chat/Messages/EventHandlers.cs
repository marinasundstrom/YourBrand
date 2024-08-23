using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

using YourBrand.ChatApp.Common;
using YourBrand.Domain;

namespace YourBrand.ChatApp.Features.Chat.Messages.EventHandlers;

public sealed class MessagePostedEventHandler : IDomainEventHandler<MessagePosted>
{
    private readonly IMessageRepository messagesRepository;
    private readonly IUserRepository userRepository;
    private readonly IChatNotificationService chatNotificationService;
    private readonly IDtoComposer dtoComposer;

    public MessagePostedEventHandler(
        IMessageRepository messagesRepository,
        IUserRepository userRepository,
        IChatNotificationService chatNotificationService,
        IDtoComposer dtoComposer)
    {
        this.messagesRepository = messagesRepository;
        this.userRepository = userRepository;
        this.chatNotificationService = chatNotificationService;
        this.dtoComposer = dtoComposer;
    }

    public async Task Handle(MessagePosted notification, CancellationToken cancellationToken)
    {
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

public sealed class MessageEditedEventHandler : IDomainEventHandler<MessageEdited>
{
    private readonly IMessageRepository messagesRepository;
    private readonly IUserRepository userRepository;
    private readonly IChatNotificationService chatNotificationService;

    public MessageEditedEventHandler(
        IMessageRepository messagesRepository,
        IUserRepository userRepository,
        IChatNotificationService chatNotificationService)
    {
        this.messagesRepository = messagesRepository;
        this.userRepository = userRepository;
        this.chatNotificationService = chatNotificationService;
    }

    public async Task Handle(MessageEdited notification, CancellationToken cancellationToken)
    {
        var message = await messagesRepository.FindByIdAsync(notification.MessageId);
        var user = await userRepository.FindByIdAsync(message!.LastEditedBy!.UserId);

        await chatNotificationService.NotifyMessageEdited(
            notification.ChannelId, new MessageEditedData(notification.MessageId, message.LastEdited.GetValueOrDefault(), new UserData(user!.Id, user.Name), notification.Content), cancellationToken);
    }
}

public sealed class MessageDeletedEventHandler : IDomainEventHandler<MessageDeleted>
{
    private readonly IChannelRepository channelRepository;
    private readonly IMessageRepository messagesRepository;
    private readonly IUserRepository userRepository;
    private readonly IChatNotificationService chatNotificationService;

    public MessageDeletedEventHandler(
        IChannelRepository channelRepository,
        IMessageRepository messagesRepository,
        IUserRepository userRepository,
        IChatNotificationService chatNotificationService)
    {
        this.channelRepository = channelRepository;
        this.messagesRepository = messagesRepository;
        this.userRepository = userRepository;
        this.chatNotificationService = chatNotificationService;
    }

    public async Task Handle(MessageDeleted notification, CancellationToken cancellationToken)
    {
        var channel = await channelRepository.FindByIdAsync(notification.ChannelId, cancellationToken);

        var shouldSoftDelete = channel.Settings.SoftDeleteMessages.GetValueOrDefault();

        if (shouldSoftDelete) 
        {
            var message = await messagesRepository.FindByIdAsync(notification.MessageId);
            var user = await userRepository.FindByIdAsync(message!.DeletedBy!.UserId);

            await chatNotificationService.NotifyMessageDeleted(
                notification.ChannelId, new MessageDeletedData(notification.MessageId, false, message!.Deleted.GetValueOrDefault(), new UserData(user!.Id, user.Name)), cancellationToken);
        }
        else 
        {
            await chatNotificationService.NotifyMessageDeleted(
                notification.ChannelId, new MessageDeletedData(notification.MessageId, true, null, null), cancellationToken);
        }
    }
}

public sealed class UserReactedToMessageEventHandler(
    IChannelRepository channelRepository,
    IMessageRepository messagesRepository,
    IUserRepository userRepository,
    IDtoFactory dtoFactory,
    IChatNotificationService chatNotificationService) : IDomainEventHandler<UserReactedToMessage>
{
    public async Task Handle(UserReactedToMessage notification, CancellationToken cancellationToken)
    {
        var message = await messagesRepository.FindByIdAsync(notification.MessageId, cancellationToken);

        var reaction = message!.Reactions.Last();

        var channel = await channelRepository.FindByIdAsync(message.ChannelId, cancellationToken);
        
        var participant = channel.Participants.FirstOrDefault(x => x.Id == notification.ParticipantId);

        var user = await userRepository.FindByIdAsync(participant!.UserId, cancellationToken);

        var reactionDto = dtoFactory.CreateReactionDto(reaction, user!, participant.Id.ToString());

        await chatNotificationService.NotifyReaction(
            notification.ChannelId, message.Id, reactionDto, cancellationToken);
    }
}

public sealed class UserRemovedReactionFromMessageEventHandler : IDomainEventHandler<UserRemovedReactionFromMessage>
{
    private readonly IChatNotificationService chatNotificationService;

    public UserRemovedReactionFromMessageEventHandler(
        IChatNotificationService chatNotificationService)
    {
        this.chatNotificationService = chatNotificationService;
    }

    public async Task Handle(UserRemovedReactionFromMessage notification, CancellationToken cancellationToken)
    {
        var reaction2 = notification.Reaction;

        await chatNotificationService.NotifyReactionRemoved(
            notification.ChannelId, notification.MessageId, reaction2, notification.ParticipantId.ToString()!, cancellationToken);
    }
}