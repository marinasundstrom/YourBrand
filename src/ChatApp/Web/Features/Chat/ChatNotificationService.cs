using ChatApp.Features.Chat.Messages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;

namespace ChatApp.Features.Chat;

public interface IChatNotificationService
{
    Task NotifyMessagePosted(MessageDto message, CancellationToken cancellationToken = default);
    Task SendMessageToUser(string userId, MessageDto message, CancellationToken cancellationToken = default);
    Task SendConfirmationToSender(Guid channelId, string senderId, Guid messageId, CancellationToken cancellationToken = default);
    Task NotifyMessageEdited(Guid channelId, MessageEditedData data, CancellationToken cancellationToken = default);
    Task NotifyMessageDeleted(Guid channelId, MessageDeletedData data, CancellationToken cancellationToken = default);

    Task NotifyReaction(Guid channelId, Guid messageId, ReactionDto reaction, CancellationToken cancellationToken = default);
    Task NotifyReactionRemoved(Guid channelId, Guid messageId, string reaction, string userId, CancellationToken cancellationToken = default);
}

public class ChatNotificationService : IChatNotificationService
{
    private readonly IHubContext<ChatHub, IChatHubClient> hubsContext;

    public ChatNotificationService(IHubContext<ChatHub, IChatHubClient> hubsContext)
    {
        this.hubsContext = hubsContext;
    }

    public async Task NotifyMessagePosted(MessageDto message, CancellationToken cancellationToken = default)
    {
        await hubsContext.Clients
            .Group($"channel-{message.ChannelId}")
            .OnMessagePosted(message.Map());
    }

    public async Task SendMessageToUser(string userId, MessageDto message, CancellationToken cancellationToken = default)
    {
        await hubsContext.Clients
            .User(userId.ToString())
            .OnMessagePosted(message.Map());
    }

    public async Task SendConfirmationToSender(Guid channelId, string senderId, Guid messageId, CancellationToken cancellationToken = default)
    {
        await hubsContext.Clients
            .User(senderId.ToString())
            .OnMessagePostedConfirmed(messageId);
    }

    public async Task NotifyMessageEdited(Guid channelId, MessageEditedData data, CancellationToken cancellationToken = default) 
    {
        await hubsContext.Clients
            .Group($"channel-{channelId}")
            .OnMessageEdited(channelId, data);
    }

    public async Task NotifyMessageDeleted(Guid channelId, MessageDeletedData data, CancellationToken cancellationToken = default) 
    {
        await hubsContext.Clients
            .Group($"channel-{channelId}")
            .OnMessageDeleted(channelId, data);
    }

    public async Task NotifyReaction(Guid channelId, Guid messageId, ReactionDto reaction, CancellationToken cancellationToken = default)
    {
        await hubsContext.Clients
            .Group($"channel-{channelId}")
            .OnMessageReaction(channelId, messageId, reaction.Map());
    }

    public async Task NotifyReactionRemoved(Guid channelId, Guid messageId, string reaction, string userId, CancellationToken cancellationToken = default)
    {
        await hubsContext.Clients
            .Group($"channel-{channelId}")
            .OnMessageReactionRemoved(channelId, messageId, reaction, userId);
    }
}