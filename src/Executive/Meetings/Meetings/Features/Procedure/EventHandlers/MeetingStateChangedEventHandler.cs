using Microsoft.AspNetCore.SignalR;

using YourBrand.Domain;
using YourBrand.Meetings.Domain.Events;
using Dtos = YourBrand.Meetings.Dtos;

namespace YourBrand.Meetings.Features.Procedure.EventHandlers;

public sealed class MeetingStateChangedEventHandler(
    IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext) : IDomainEventHandler<MeetingStateChanged>
{
    private readonly IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> _hubContext = hubContext;

    public async Task Handle(MeetingStateChanged notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients
            .Group($"meeting-{notification.MeetingId.Value}")
            .OnMeetingStateChanged((Dtos.MeetingState)notification.State, notification.AdjournmentMessage);
    }
}
