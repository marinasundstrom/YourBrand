using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Procedure.Command;

public sealed record CancelMeeting(string OrganizationId, int Id) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext, IHubContext<MeetingsProcedureHub, IMeetingsProcedureHubClient> hubContext) : IRequestHandler<CancelMeeting, Result>
    {
        public async Task<Result> Handle(CancelMeeting request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (meeting is null)
            {
                return Errors.Meetings.MeetingNotFound;
            }

            var participant = meeting.Participants.FirstOrDefault(x => x.UserId == userContext.UserId);

            if (participant is null)
            {
                return Errors.Meetings.YouAreNotParticipantOfMeeting;
            }

            if (participant.Role != ParticipantRole.Chairperson)
            {
                return Errors.Meetings.OnlyChairpersonCanEndTheMeeting;
            }

            meeting.CancelMeeting();

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            await hubContext.Clients
                .Group($"meeting-{meeting.Id}")
                .OnMeetingStateChanged();

            return Result.Success;
        }
    }
}