using MediatR;
using YourBrand.Meetings.Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.SignalR;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Minutes;

namespace YourBrand.Meetings.Features.Procedure.Command;

public sealed record StartMeeting(string OrganizationId, int Id) : IRequest<Result>
{
    public sealed class Handler(
        IApplicationDbContext context,
        IUserContext userContext,
        IHubContext<SecretaryHub, ISecretaryHubClient> secretaryHubContext) : IRequestHandler<StartMeeting, Result>
    {
        public async Task<Result> Handle(StartMeeting request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .Include(x => x.Minutes)
                .Include(x => x.Agenda!)
                .ThenInclude(a => a.Items)
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

            var chairFunction = meeting.GetChairpersonFunction(attendee);

            if (chairFunction is null)
            {
                return Errors.Meetings.OnlyChairpersonCanStartTheMeeting;
            }

            chairFunction.StartMeeting();

            await context.EnsureMinutesAsync(meeting, cancellationToken);

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            await secretaryHubContext.Clients
                .Group($"meeting-{meeting.Id}")
                .OnMinutesUpdated();

            return Result.Success;
        }
    }
}