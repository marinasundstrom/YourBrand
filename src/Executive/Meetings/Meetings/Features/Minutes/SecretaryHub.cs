using System.Collections.Concurrent;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Features.Minutes;

public record ConnectionState(string TenantId, string OrganizationId, int MeetingId);

[Authorize]
public sealed class SecretaryHub(IMediator mediator, ISettableUserContext userContext, ISettableTenantContext tenantContext) : Hub<ISecretaryHubClient>, ISecretaryHub
{
    private readonly static ConcurrentDictionary<string, ConnectionState> state = new ConcurrentDictionary<string, ConnectionState>();

    public override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext is not null)
        {
            var tenantId = httpContext?.User?.FindFirst("tenant_id")?.Value;

            if (httpContext.Request.Query.TryGetValue("organizationId", out var organizationId))
            {

            }

            if (httpContext.Request.Query.TryGetValue("meetingId", out var meetingId))
            {
                Groups.AddToGroupAsync(this.Context.ConnectionId, $"meeting-{meetingId}");
            }

            state.TryAdd(Context.ConnectionId, new ConnectionState(tenantId, organizationId, int.Parse(meetingId)));
        }

        return base.OnConnectedAsync();
    }

    public async Task ChangeMinutesItem(string minutesItemId)
    {
        var s = state[Context.ConnectionId];

        tenantContext.SetTenantId(s.TenantId);
        userContext.SetCurrentUser(Context.User!);
        userContext.SetConnectionId(Context.ConnectionId);

        //return (string)await mediator.Send(
        //    new PostMessage(s.OrganizationId, channelId, replyTo, content));
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        state.Remove(Context.ConnectionId, out var _);

        return base.OnDisconnectedAsync(exception);
    }
}