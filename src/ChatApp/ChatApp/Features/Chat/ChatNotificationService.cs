using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;

using YourBrand.ChatApp.Features.Chat.Messages;

namespace YourBrand.ChatApp.Features.Chat;

public interface IChatNotificationService
{
    Task NotifyMessagePosted(MessageDto message, CancellationToken cancellationToken = default);
    Task SendMessageToUser(string userId, MessageDto message, CancellationToken cancellationToken = default);
    Task SendConfirmationToSender(string channelId, string senderId, string messageId, CancellationToken cancellationToken = default);
    Task NotifyMessageEdited(string channelId, MessageEditedData data, CancellationToken cancellationToken = default);
    Task NotifyMessageDeleted(string channelId, MessageDeletedData data, CancellationToken cancellationToken = default);

    Task NotifyReaction(string channelId, string messageId, ReactionDto reaction, CancellationToken cancellationToken = default);
    Task NotifyReactionRemoved(string channelId, string messageId, string reaction, string userId, CancellationToken cancellationToken = default);
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

    public async Task SendConfirmationToSender(string channelId, string senderId, string messageId, CancellationToken cancellationToken = default)
    {
        await hubsContext.Clients
            .User(senderId.ToString())
            .OnMessagePostedConfirmed(messageId);
    }

    public async Task NotifyMessageEdited(string channelId, MessageEditedData data, CancellationToken cancellationToken = default)
    {
        await hubsContext.Clients
            .Group($"channel-{channelId}")
            .OnMessageEdited(channelId, data);
    }

    public async Task NotifyMessageDeleted(string channelId, MessageDeletedData data, CancellationToken cancellationToken = default)
    {
        await hubsContext.Clients
            .Group($"channel-{channelId}")
            .OnMessageDeleted(channelId, data);
    }

    public async Task NotifyReaction(string channelId, string messageId, ReactionDto reaction, CancellationToken cancellationToken = default)
    {
        await hubsContext.Clients
            .Group($"channel-{channelId}")
            .OnMessageReaction(channelId, messageId, reaction.Map());
    }

    public async Task NotifyReactionRemoved(string channelId, string messageId, string reaction, string participantId, CancellationToken cancellationToken = default)
    {
        await hubsContext.Clients
            .Group($"channel-{channelId}")
            .OnMessageReactionRemoved(channelId, messageId, reaction, participantId);
    }
}