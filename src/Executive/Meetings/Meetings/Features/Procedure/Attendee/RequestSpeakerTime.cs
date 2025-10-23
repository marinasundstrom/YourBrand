using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Procedure.Command;
using YourBrand.Meetings.Features.Procedure.Discussions;

namespace YourBrand.Meetings.Features.Procedure.Attendee;

public sealed record RequestSpeakerTime(string OrganizationId, int Id, string AgendaItemId) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext, IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext) : IRequestHandler<RequestSpeakerTime, Result>
    {
        public async Task<Result> Handle(RequestSpeakerTime request, CancellationToken cancellationToken)
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

            if (!meeting.IsAttendeeAllowedToSpeak(attendee))
            {
                return Errors.Meetings.YouHaveNoSpeakingRights;
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

            if (agendaItem.Discussion.SpeakingTimeLimit is null)
            {
                return Errors.Meetings.SpeakingTimeNotConfigured;
            }

            var speakerRequest = agendaItem.Discussion!.AddSpeakerRequest(attendee);

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            await hubContext.Clients
               .Group($"meeting-{meeting.Id}")
               .OnSpeakerRequestAdded(agendaItem.Id, speakerRequest.Id, speakerRequest.AttendeeId, speakerRequest.Name);

            await hubContext.Clients
               .Group($"meeting-{meeting.Id}")
               .OnSpeakerTimeExtended(agendaItem.Id, speakerRequest.Id, (int?)speakerRequest.AllocatedSpeakingTime?.TotalSeconds);

            return Result.Success;
        }
    }
}