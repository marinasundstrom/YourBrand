using System;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;
using YourBrand.Meetings.Features.Procedure.Command;

namespace YourBrand.Meetings.Features.Procedure.Chairman;

public sealed record ResetSpeakerClock(string OrganizationId, int Id, string AgendaItemId) : IRequest<Result>
{
    public sealed class Handler(
        IApplicationDbContext context,
        IUserContext userContext,
        IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext)
        : IRequestHandler<ResetSpeakerClock, Result>
    {
        public async Task<Result> Handle(ResetSpeakerClock request, CancellationToken cancellationToken)
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

            if (attendee.Role != AttendeeRole.Chairperson)
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

            agendaItem.Discussion.ResetCurrentSpeakerClock();

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            var now = DateTimeOffset.UtcNow;
            var snapshot = agendaItem.Discussion.GetCurrentSpeakerClockSnapshot(now);
            var currentSpeakerId = agendaItem.Discussion.CurrentSpeaker!.Id.Value;
            var elapsedSeconds = (int)Math.Max(0, Math.Round(snapshot.Elapsed.TotalSeconds));

            await hubContext.Clients
                .Group($"meeting-{meeting.Id}")
                .OnSpeakerClockReset(agendaItem.Id, currentSpeakerId, elapsedSeconds);

            return Result.Success;
        }
    }
}
