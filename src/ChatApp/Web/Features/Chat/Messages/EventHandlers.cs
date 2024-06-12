using ChatApp.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace ChatApp.Features.Chat.Messages.EventHandlers;

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

        var user = await userRepository.FindByIdAsync(message.CreatedById.GetValueOrDefault(), cancellationToken);

        if (user is null)
            return;

        await NotifyChannel(message, user, cancellationToken);
    }

    private async Task SendConfirmationToSender(Message message, CancellationToken cancellationToken)
    {
        await chatNotificationService.SendConfirmationToSender(
            message.ChannelId, message.CreatedById.GetValueOrDefault(), message.Id, cancellationToken);
    }

    private async Task NotifyChannel(Message message, User user, CancellationToken cancellationToken)
    {
        MessageDto messageDto = await dtoComposer.ComposeMessageDto(message,cancellationToken);

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
        var user = await userRepository.FindByIdAsync(message!.LastModifiedById.GetValueOrDefault());

        await chatNotificationService.NotifyMessageEdited(
            notification.ChannelId, new MessageEditedData(notification.MessageId, message.LastModified.GetValueOrDefault(), new UserData(user!.Id, user.Name), notification.Content), cancellationToken);
    }
}

public sealed class MessageDeletedEventHandler : IDomainEventHandler<MessageDeleted>
{
    private readonly IMessageRepository messagesRepository;
    private readonly IUserRepository userRepository;
    private readonly IChatNotificationService chatNotificationService;

    public MessageDeletedEventHandler(
        IMessageRepository messagesRepository, 
        IUserRepository userRepository,
        IChatNotificationService chatNotificationService)
    {
        this.messagesRepository = messagesRepository;
        this.userRepository = userRepository;
        this.chatNotificationService = chatNotificationService;
    }

    public async Task Handle(MessageDeleted notification, CancellationToken cancellationToken)
    {
        var message = await messagesRepository.FindByIdAsync(notification.MessageId);
        var user = await userRepository.FindByIdAsync(message!.DeletedById.GetValueOrDefault());

        await chatNotificationService.NotifyMessageDeleted(
            notification.ChannelId, new MessageDeletedData(notification.MessageId, message!.Deleted.GetValueOrDefault(), new UserData(user!.Id, user.Name)), cancellationToken);
    }
}

public sealed class UserReactedToMessageEventHandler : IDomainEventHandler<UserReactedToMessage>
{
    private readonly IMessageRepository messagesRepository;
    private readonly IUserRepository userRepository;
    private readonly IDtoFactory dtoFactory;
    private readonly IChatNotificationService chatNotificationService;

    public UserReactedToMessageEventHandler(
        IMessageRepository messagesRepository, 
        IUserRepository userRepository,
        IDtoFactory dtoFactory,
        IChatNotificationService chatNotificationService)
    {
        this.messagesRepository = messagesRepository;
        this.userRepository = userRepository;
        this.dtoFactory = dtoFactory;
        this.chatNotificationService = chatNotificationService;
    }

    public async Task Handle(UserReactedToMessage notification, CancellationToken cancellationToken)
    {
        var message = await messagesRepository.FindByIdAsync(notification.MessageId);

        var reaction = message!.Reactions.Last();

        var user = await userRepository.FindByIdAsync(reaction!.UserId);

        var reactionDto = dtoFactory.CreateReactionDto(reaction, user!);

        await chatNotificationService.NotifyReaction(
            notification.ChannelId, message.Id, reactionDto, cancellationToken);
    }
}

public sealed class UserRemovedReactionFromMessageEventHandler : IDomainEventHandler<UserRemovedReactionFromMessage>
{
    private readonly IMessageRepository messagesRepository;
    private readonly IChatNotificationService chatNotificationService;

    public UserRemovedReactionFromMessageEventHandler(
        IMessageRepository messagesRepository, 
        IChatNotificationService chatNotificationService)
    {
        this.messagesRepository = messagesRepository;
        this.chatNotificationService = chatNotificationService;
    }

    public async Task Handle(UserRemovedReactionFromMessage notification, CancellationToken cancellationToken)
    {
        var message = await messagesRepository.FindByIdAsync(notification.MessageId);

        var reaction2 = notification.Reaction;

        await chatNotificationService.NotifyReactionRemoved(
            notification.ChannelId, notification.MessageId, reaction2, notification.CurrentUserId.ToString()!, cancellationToken);
    }
}
