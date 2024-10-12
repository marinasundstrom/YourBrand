using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Features.Procedure.Command;

public record ConnectionState(string TenantId, string OrganizationId, string MeetingId);

[Authorize]
public sealed class MeetingsProcedureHub : Hub<IMeetingsProcedureHubClient>, IMeetingsProcedureHub
{
    private readonly IMediator mediator;
    private readonly ISettableUserContext userContext;
    private readonly ISettableTenantContext tenantContext;
    private readonly static Dictionary<string, ConnectionState> state = new Dictionary<string, ConnectionState>(); 

    public MeetingsProcedureHub(IMediator mediator, ISettableUserContext userContext, ISettableTenantContext tenantContext)
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

            if(httpContext.Request.Query.TryGetValue("organizationId", out var organizationId)) 
            {

            }

            if (httpContext.Request.Query.TryGetValue("meetingId", out var meetingId))
            {
                Groups.AddToGroupAsync(this.Context.ConnectionId, $"meeting-{meetingId}");
            }

            state.Add(Context.ConnectionId, new ConnectionState(tenantId, organizationId, meetingId));
        }

        return base.OnConnectedAsync();
    }

    public async Task ChangeAgendaItem(string agendaItemId)
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
        state.Remove(Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
}