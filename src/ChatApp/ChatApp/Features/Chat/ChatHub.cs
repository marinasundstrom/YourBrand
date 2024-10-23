using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.ChatApp.Features.Chat.Messages;
using YourBrand.Tenancy;

namespace YourBrand.ChatApp.Features.Chat;

public record ConnectionState(string TenantId, string OrganizationId, string ChannelId);

[Authorize]
public sealed class ChatHub : Hub<IChatHubClient>, IChatHub
{
    private readonly IMediator mediator;
    private readonly ISettableUserContext userContext;
    private readonly ISettableTenantContext tenantContext;
    private readonly static Dictionary<string, ConnectionState> state = new Dictionary<string, ConnectionState>();

    public ChatHub(IMediator mediator, ISettableUserContext userContext, ISettableTenantContext tenantContext)
    {
        this.mediator = mediator;
        this.userContext = userContext;
        this.tenantContext = tenantContext;
    }

    public override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext is not null)
        {
            var tenantId = httpContext?.User?.FindFirst("tenant_id")?.Value;

            if (httpContext.Request.Query.TryGetValue("organizationId", out var organizationId))
            {

            }

            if (httpContext.Request.Query.TryGetValue("channelId", out var channelId))
            {
                Groups.AddToGroupAsync(this.Context.ConnectionId, $"channel-{channelId}");
            }

            state.Add(Context.ConnectionId, new ConnectionState(tenantId, organizationId, channelId));
        }

        return base.OnConnectedAsync();
    }

    public async Task<string> PostMessage(string channelId, string? replyTo, string content)
    {
        var s = state[Context.ConnectionId];

        tenantContext.SetTenantId(s.TenantId);
        userContext.SetCurrentUser(Context.User!);
        userContext.SetConnectionId(Context.ConnectionId);

        Console.WriteLine($"Tenant Id: {s.TenantId}");
        Console.WriteLine($"Organization Id: {s.OrganizationId}");

        return (string)await mediator.Send(
            new PostMessage(s.OrganizationId, channelId, replyTo, content));
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        state.Remove(Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }

    /*

    public async Task EditMessage(string channelId, string messageId, string content) 
    {
        userContext.SetCurrentUser(Context.User!);
        
        var senderId = Context.UserIdentifier!;

        await Clients
            .Group($"channel-{channelId}")
            //.GroupExcept($"channel-{channelId}", Context.ConnectionId)
            .MessageEdited(channelId, new MessageEdited(messageId, content);
    }

    public async Task DeleteMessage(string channelId, string messageId) 
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