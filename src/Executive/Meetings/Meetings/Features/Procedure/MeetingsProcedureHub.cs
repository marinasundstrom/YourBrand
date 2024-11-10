using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Procedure.Attendee;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Features.Procedure;

public record ConnectionState(string TenantId, string OrganizationId, int MeetingId);

[Authorize]
public sealed class MeetingsProcedureHub(IMediator mediator, ISettableUserContext userContext, ISettableTenantContext tenantContext) : Hub<IMeetingsProcedureHubClient>, IMeetingsProcedureHub
{
    private readonly static Dictionary<string, ConnectionState> state = new Dictionary<string, ConnectionState>();

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
                Groups.AddToGroupAsync(this.Context.ConnectionId, GetChannelName(meetingId.First()));
            }

            state.Add(Context.ConnectionId, new ConnectionState(tenantId, organizationId, int.Parse(meetingId)));
        }

        return base.OnConnectedAsync();
    }

    public async Task ChangeAgendaItem(string agendaItemId)
    {
        var connectionState = state[Context.ConnectionId];

        SetContext(userContext, tenantContext, connectionState);

        //return (string)await mediator.Send(
        //    new PostMessage(s.OrganizationId, channelId, replyTo, content));
    }

    public async Task RequestSpeakerTime(string agendaItemId)
    {
        var connectionState = state[Context.ConnectionId];

        SetContext(userContext, tenantContext, connectionState);

        await mediator.Send(
            new RequestSpeakerTime(connectionState.OrganizationId, connectionState.MeetingId, agendaItemId));
    }

    public async Task RevokeSpeakerTime(string agendaItemId)
    {
        var connectionState = state[Context.ConnectionId];

        SetContext(userContext, tenantContext, connectionState);

        await mediator.Send(
              new RevokeSpeakerTime(connectionState.OrganizationId, connectionState.MeetingId, agendaItemId));
    }

    private void SetContext(ISettableUserContext userContext, ISettableTenantContext tenantContext, ConnectionState s)
    {
        tenantContext.SetTenantId(s.TenantId);
        userContext.SetCurrentUser(Context.User!);
        userContext.SetConnectionId(Context.ConnectionId);
    }

    /*
    public async  Task VotingStatusChanged(int status)
    {
        var connectionState = state[Context.ConnectionId];

        var group = GetGroup(connectionState);

        await group.OnVotingStatusChanged(status);
    }
    */

    private IMeetingsProcedureHubClient GetGroup(ConnectionState connectionState)
    {
        var group = Clients.Group(GetChannelName(connectionState.MeetingId.ToString()));
        return group;
    }

    private static string GetChannelName(string meetingId)
    {
        return $"meeting-{meetingId}";
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        state.Remove(Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
}