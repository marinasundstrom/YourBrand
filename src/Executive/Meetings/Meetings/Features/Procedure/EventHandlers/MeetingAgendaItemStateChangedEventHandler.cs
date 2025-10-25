using Microsoft.AspNetCore.SignalR;

using YourBrand.Domain;
using YourBrand.Meetings.Domain.Events;
using Dtos = YourBrand.Meetings.Dtos;

namespace YourBrand.Meetings.Features.Procedure.EventHandlers;

public sealed class MeetingAgendaItemStateChangedEventHandler(
    IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext) : IDomainEventHandler<MeetingAgendaItemStateChanged>
{
    private readonly IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> _hubContext = hubContext;

    public async Task Handle(MeetingAgendaItemStateChanged notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group($"meeting-{notification.MeetingId.Value}")
            .OnAgendaItemStateChanged(notification.AgendaItemId.Value, (Dtos.AgendaItemState)notification.State, (Dtos.AgendaItemPhase)notification.Phase);
    }
}
