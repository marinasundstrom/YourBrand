using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Procedure.Command;

public sealed record StartMeeting(string OrganizationId, int Id) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<StartMeeting, Result>
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

            var participant = meeting.Participants.FirstOrDefault(x => x.UserId == userContext.UserId);

            if (participant is null)
                throw new UnauthorizedAccessException("You are not a participant of this meeting.");

            if (participant.Role != ParticipantRole.Chairperson)
                throw new UnauthorizedAccessException("Only the Chairperson can start the meeting.");

            meeting.StartMeeting();
            
            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
