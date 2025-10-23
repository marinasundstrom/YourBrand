using MediatR;
using YourBrand.Meetings.Domain.Entities;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;
using YourBrand.Meetings.Features.Procedure.Command;
using YourBrand.Meetings.Features.Procedure.Discussions;

namespace YourBrand.Meetings.Features.Procedure.Chairman;

public sealed record SetDiscussionSpeakingTime(string OrganizationId, int Id, string AgendaItemId, int? SpeakingTimeLimitSeconds) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext, IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext)
        : IRequestHandler<SetDiscussionSpeakingTime, Result>
    {
        public async Task<Result> Handle(SetDiscussionSpeakingTime request, CancellationToken cancellationToken)
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

            agendaItem.Discussion ??= new Discussion
            {
                OrganizationId = meeting.OrganizationId,
                TenantId = meeting.TenantId,
                AgendaItemId = agendaItem.Id
            };

            TimeSpan? speakingTime = request.SpeakingTimeLimitSeconds is null
                ? null
                : TimeSpan.FromSeconds(request.SpeakingTimeLimitSeconds.Value);

            try
            {
                agendaItem.Discussion.SetSpeakingTimeLimit(speakingTime);
            }
            catch (InvalidOperationException)
            {
                return Errors.Meetings.InvalidSpeakingTimeLimit;
            }

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            await hubContext.Clients
                .Group($"meeting-{meeting.Id}")
                .OnDiscussionSpeakingTimeChanged(agendaItem.Id, request.SpeakingTimeLimitSeconds);

            return Result.Success;
        }
    }
}
