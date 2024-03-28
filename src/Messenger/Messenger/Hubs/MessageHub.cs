
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Contracts;

using MassTransit;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using YourBrand.Identity;

namespace YourBrand.Messenger.Hubs;

[Authorize(AuthenticationSchemes = Messenger.Authentication.AuthSchemes.Default)]
public class MessageHub : Hub<IMessageClient>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestClient<PostMessage> _postMessageClient;
    private readonly IBus _bus;

    public MessageHub(IMediator mediator, ICurrentUserService currentUserService,
        IRequestClient<PostMessage> postMessageClient,
        IBus bus)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _postMessageClient = postMessageClient;
        _bus = bus;
    }

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
        _currentUserService.SetCurrentUser(this.Context.User!);

        var response = await _postMessageClient.GetResponse<MessageDto>(new PostMessage(_currentUserService.GetAccessToken()!, conversationId, text, replyToId));
    }

    public async Task MessageRead(string conversationId, string id)
    {
        _currentUserService.SetCurrentUser(this.Context.User!);

        await _bus.Publish(new MarkMessageAsRead(_currentUserService.GetAccessToken()!, conversationId, id));
    }

    public async Task EditMessage(string conversationId, string id, string text) 
    {
        _currentUserService.SetCurrentUser(this.Context.User!);

        await _bus.Publish(new UpdateMessage(_currentUserService.GetAccessToken()!, conversationId, id, text));
    }

    public async Task DeleteMessage(string conversationId, string id) 
    {
        _currentUserService.SetCurrentUser(this.Context.User!);

        await _bus.Publish(new DeleteMessage(_currentUserService.GetAccessToken()!, conversationId, id));
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