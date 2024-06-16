using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.ChatApp.Features.Chat.Messages;

namespace YourBrand.ChatApp.Features.Chat;

[Authorize]
public sealed class ChatHub : Hub<IChatHubClient>, IChatHub
{
    private readonly IMediator mediator;
    private readonly ISettableUserContext userContext;

    public ChatHub(IMediator mediator, ISettableUserContext userContext)
    {
        this.mediator = mediator;
        this.userContext = userContext;
    }

    public override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext is not null)
        {
            if (httpContext.Request.Query.TryGetValue("channelId", out var channelId))
            {
                Groups.AddToGroupAsync(this.Context.ConnectionId, $"channel-{channelId}");
            }
        }

        return base.OnConnectedAsync();
    }

    public async Task<Guid> PostMessage(Guid channelId, Guid? replyTo, string content)
    {
        userContext.SetCurrentUser(Context.User!);
        userContext.SetConnectionId(Context.ConnectionId);

        return (MessageId)await mediator.Send(
            new PostMessage(channelId, replyTo, content));
    }

    /*

    public async Task EditMessage(Guid channelId, Guid messageId, string content) 
    {
        userContext.SetCurrentUser(Context.User!);
        
        var senderId = Context.UserIdentifier!;

        await Clients
            .Group($"channel-{channelId}")
            //.GroupExcept($"channel-{channelId}", Context.ConnectionId)
            .MessageEdited(channelId, new MessageEdited(messageId, content);
    }

    public async Task DeleteMessage(Guid channelId, Guid messageId) 
    {
        userContext.SetCurrentUser(Context.User!);

        var senderId = Context.UserIdentifier!;

        await Clients
            .Group($"channel-{channelId}")
            //.GroupExcept($"channel-{channelId}", Context.ConnectionId)
            .MessageDeleted(channelId, messageId);
    }

    */
}