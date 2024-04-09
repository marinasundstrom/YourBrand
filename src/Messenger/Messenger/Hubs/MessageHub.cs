
using MassTransit;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using YourBrand.Identity;
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Contracts;

namespace YourBrand.Messenger.Hubs;

[Authorize(AuthenticationSchemes = Messenger.Authentication.AuthSchemes.Default)]
public class MessageHub(IMediator mediator, ISettableUserContext userContext,
    IRequestClient<PostMessage> postMessageClient,
    IBus bus) : Hub<IMessageClient>
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        var httpContext = Context.GetHttpContext();

        if (httpContext is not null)
        {
            if (httpContext.Request.Query.TryGetValue("conversationId", out var paymentId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation-{paymentId}");
            }
        }

        await Clients.Others.UserJoined(new UserDto2()
        {
            User = this.Context.User!.FindFirst(x => x.Type.EndsWith("name"))!.Value,
            UserId = this.Context.UserIdentifier
        });
    }

    public async Task SendMessage(string conversationId, string text, string? replyToId)
    {
        userContext.SetCurrentUser(this.Context.User!);

        var response = await postMessageClient.GetResponse<MessageDto>(new PostMessage(userContext.GetAccessToken()!, conversationId, text, replyToId));
    }

    public async Task MessageRead(string conversationId, string id)
    {
        userContext.SetCurrentUser(this.Context.User!);

        await bus.Publish(new MarkMessageAsRead(userContext.GetAccessToken()!, conversationId, id));
    }

    public async Task EditMessage(string conversationId, string id, string text)
    {
        userContext.SetCurrentUser(this.Context.User!);

        await bus.Publish(new UpdateMessage(userContext.GetAccessToken()!, conversationId, id, text));
    }

    public async Task DeleteMessage(string conversationId, string id)
    {
        userContext.SetCurrentUser(this.Context.User!);

        await bus.Publish(new DeleteMessage(userContext.GetAccessToken()!, conversationId, id));
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        /*
        await Clients.Others.UserLeft(new UserDto2()
        {
            User = this.Context.User!.FindFirst(x => x.Type.EndsWith("name"))!.Value,
            UserId = this.Context.UserIdentifier
        });
        */

        await base.OnDisconnectedAsync(exception);
    }
}