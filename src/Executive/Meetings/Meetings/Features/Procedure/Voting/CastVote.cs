using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Procedure.Voting;

public sealed record CastVote(string OrganizationId, int Id, VoteOption Option) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<CastVote, Result>
    {
        public async Task<Result> Handle(CastVote request, CancellationToken cancellationToken)
        {
            var meeting = await context.Meetings
                .InOrganization(request.OrganizationId)
                .Include(x => x.Agenda)
                .ThenInclude(x => x.Items.OrderBy(x => x.Order ))
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

            if (!meeting.IsAttendeeAllowedToVote(attendee))
            {
                return Errors.Meetings.YouHaveNoVotingRights;
            }

            var agendaItem = meeting.GetCurrentAgendaItem();

            if (agendaItem is null)
            {
                return Errors.Meetings.NoActiveAgendaItem;
            }

            if (agendaItem.VotingSession is null)
            {
                return Errors.Meetings.NoOngoingVotingSession;
            }

            agendaItem.VotingSession!.AddVote(new Vote
            {
                OrganizationId = request.OrganizationId,
                VoterId = attendee.Id,
                Option = request.Option,
                TimeCast = DateTimeOffset.UtcNow
            });

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
