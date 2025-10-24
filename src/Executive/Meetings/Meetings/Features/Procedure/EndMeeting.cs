using MediatR;
using YourBrand.Meetings.Domain.Entities;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Procedure.Command;

public sealed record EndMeeting(string OrganizationId, int Id) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<EndMeeting, Result>
    {
        public async Task<Result> Handle(EndMeeting request, CancellationToken cancellationToken)
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
                return Errors.Meetings.OnlyChairpersonCanEndTheMeeting;
            }

            meeting.EndMeeting();

            var minutes = meeting.Minutes;

            if (minutes is null)
            {
                minutes = await context.Minutes
                    .InOrganization(request.OrganizationId)
                    .Include(x => x.Tasks)
                    .FirstOrDefaultAsync(x => x.MeetingId == meeting.Id, cancellationToken);
            }

            if (minutes is not null)
            {
                minutes.EnsurePostMeetingWorkflowTasks(meeting);
                minutes.UpdateStateFromTasks();

                context.Minutes.Update(minutes);
            }

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}