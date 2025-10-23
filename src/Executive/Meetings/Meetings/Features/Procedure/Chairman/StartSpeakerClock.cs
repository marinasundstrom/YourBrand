using System;
using YourBrand.Meetings.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;
using YourBrand.Meetings.Features.Procedure.Command;

namespace YourBrand.Meetings.Features.Procedure.Chairman;

public sealed record StartSpeakerClock(string OrganizationId, int Id, string AgendaItemId) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext, IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext)
        : IRequestHandler<StartSpeakerClock, Result>
    {
        public async Task<Result> Handle(StartSpeakerClock request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .Include(x => x.Agenda)
                .ThenInclude(x => x.Items.OrderBy(x => x.Order))
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            var attendee = meeting.GetAttendeeByUserId(userContext.UserId);

            if (attendee is null)
            {
                return Errors.Meetings.YouAreNotAttendeeOfMeeting;
            }

            if (!meeting.CanAttendeeActAsChair(attendee))
            {
                return Errors.Meetings.OnlyChairpersonCanManageSpeakerQueue;
            }

            var agendaItem = meeting.GetAgendaItem(request.AgendaItemId);

            if (agendaItem is null)
            {
                return Errors.Meetings.AgendaItemNotFound;
            }

            if (agendaItem.Discussion is null)
            {
                return Errors.Meetings.NoOngoingDiscussionSession;
            }

            if (agendaItem.Discussion.CurrentSpeaker is null)
            {
                return Errors.Meetings.NoCurrentSpeaker;
            }

            if (agendaItem.Discussion.IsCurrentSpeakerClockRunning)
            {
                return Errors.Meetings.SpeakerClockAlreadyRunning;
            }

            var now = DateTimeOffset.UtcNow;

            agendaItem.Discussion.StartCurrentSpeakerClock(now);

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            var snapshot = agendaItem.Discussion.GetCurrentSpeakerClockSnapshot(now);
            var currentSpeakerId = agendaItem.Discussion.CurrentSpeaker!.Id.Value;
            var elapsedSeconds = (int)Math.Max(0, Math.Round(snapshot.Elapsed.TotalSeconds));
            var startedAtUtc = snapshot.StartedAtUtc ?? now;

            await hubContext.Clients
                .Group($"meeting-{meeting.Id}")
                .OnSpeakerClockStarted(agendaItem.Id, currentSpeakerId, elapsedSeconds, startedAtUtc);

            return Result.Success;
        }
    }
}
