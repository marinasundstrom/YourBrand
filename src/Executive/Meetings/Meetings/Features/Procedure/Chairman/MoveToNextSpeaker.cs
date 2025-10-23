using System;
using YourBrand.Meetings.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;
using YourBrand.Meetings.Features.Procedure.Command;
using YourBrand.Meetings.Features.Procedure.Discussions;

namespace YourBrand.Meetings.Features.Procedure.Chairman;

public sealed record MoveToNextSpeaker(string OrganizationId, int Id) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext, IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext) : IRequestHandler<MoveToNextSpeaker, Result>
    {
        public async Task<Result> Handle(MoveToNextSpeaker request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .Include(x => x.Agenda)
                .ThenInclude(x => x.Items.OrderBy(x => x.Order))
                .FirstOrDefaultAsync(x => x.Id == request.Id);

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

            var agendaItem = meeting.GetCurrentAgendaItem();

            if (agendaItem is null)
            {
                return Errors.Meetings.AgendaItemNotFound;
            }

            if (agendaItem.Discussion is null)
            {
                return Errors.Meetings.NoOngoingDiscussionSession;
            }

            var transition = agendaItem.Discussion!.MoveToNextSpeaker();

            var nextSpeakerId = transition.CurrentSpeaker is null ? null : transition.CurrentSpeaker.Id.Value;
            var previousSpeakerId = transition.PreviousSpeaker is null ? null : transition.PreviousSpeaker.Id.Value;
            var previousElapsedSeconds = transition.PreviousElapsed.HasValue
                ? (int)Math.Max(0, Math.Round(transition.PreviousElapsed.Value.TotalSeconds))
                : 0;

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            if (previousSpeakerId is not null)
            {
                await hubContext.Clients
                    .Group($"meeting-{meeting.Id}")
                    .OnSpeakerClockStopped(agendaItem.Id, previousSpeakerId, previousElapsedSeconds);
            }

            await hubContext.Clients
               .Group($"meeting-{meeting.Id}")
               .OnMovedToNextSpeaker(agendaItem.Id, nextSpeakerId);

            return Result.Success;
        }
    }
}