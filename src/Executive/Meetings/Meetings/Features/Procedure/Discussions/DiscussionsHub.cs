using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Features.Procedure.Discussions;

public record ConnectionState(string TenantId, string OrganizationId, int MeetingId);

[Authorize]
public sealed class DiscussionsHub(IMediator mediator, ISettableUserContext userContext, ISettableTenantContext tenantContext) : Hub<IDiscussionsHubClient>, IDiscussionsHub
{
    private readonly static Dictionary<string, ConnectionState> state = new Dictionary<string, ConnectionState>();

    public override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext is not null)
        {
            var tenantId = httpContext?.User?.FindFirst("tenant_id")?.Value;

            if(httpContext.Request.Query.TryGetValue("organizationId", out var organizationId)) 
            {

            }

            if (httpContext.Request.Query.TryGetValue("meetingId", out var meetingId))
            {
                Groups.AddToGroupAsync(this.Context.ConnectionId, $"meeting-{meetingId}");
            }

            state.Add(Context.ConnectionId, new ConnectionState(tenantId, organizationId, int.Parse(meetingId)));
        }

        return base.OnConnectedAsync();
    }

    public async Task RequestSpeakerTime(string agendaItemId)
    {
        var s = state[Context.ConnectionId];

        tenantContext.SetTenantId(s.TenantId);
        userContext.SetCurrentUser(Context.User!);
        userContext.SetConnectionId(Context.ConnectionId);

        await mediator.Send(
            new RequestSpeakerTime(s.OrganizationId, s.MeetingId, agendaItemId));    
    }

    public async Task RevokeSpeakerTime(string agendaItemId)
    {
        var s = state[Context.ConnectionId];

        tenantContext.SetTenantId(s.TenantId);
        userContext.SetCurrentUser(Context.User!);
        userContext.SetConnectionId(Context.ConnectionId);

        await mediator.Send(
              new RevokeSpeakerTime(s.OrganizationId, s.MeetingId, agendaItemId));
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        state.Remove(Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
}