using Microsoft.AspNetCore.SignalR;

using YourBrand.Domain;
using YourBrand.Meetings.Domain.Events;

namespace YourBrand.Meetings.Features.Procedure.EventHandlers;

public sealed class MeetingAgendaItemChangedEventHandler(
    IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext) : IDomainEventHandler<MeetingAgendaItemChanged>
{
    private readonly IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> _hubContext = hubContext;

    public async Task Handle(MeetingAgendaItemChanged notification, CancellationToken cancellationToken)
    {
        if (notification.AgendaItemId is null)
        {
            return;
        }

        await _hubContext.Clients
            .Group($"meeting-{notification.MeetingId.Value}")
            .OnAgendaItemChanged(notification.AgendaItemId);
    }
}
