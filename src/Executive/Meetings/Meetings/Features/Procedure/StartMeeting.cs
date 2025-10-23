using MediatR;
using YourBrand.Meetings.Domain.Entities;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Procedure.Command;

public sealed record StartMeeting(string OrganizationId, int Id) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext, IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext) : IRequestHandler<StartMeeting, Result>
    {
        public async Task<Result> Handle(StartMeeting request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
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
                return Errors.Meetings.OnlyChairpersonCanStartTheMeeting;
            }

            meeting.StartMeeting();

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            await hubContext.Clients
                .Group($"meeting-{meeting.Id}")
                .OnMeetingStateChanged((Dtos.MeetingState)meeting.State, meeting.AdjournmentMessage);

            await hubContext.Clients
                .Group($"meeting-{meeting.Id}")
                .OnAgendaItemChanged(meeting.GetCurrentAgendaItem().Id);

            return Result.Success;
        }
    }
}