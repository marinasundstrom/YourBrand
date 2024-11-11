using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Procedure.Attendee;

public sealed record WithdrawCandidacy(string OrganizationId, int Id, string CandidateId) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<WithdrawCandidacy, Result>
    {
        public async Task<Result> Handle(WithdrawCandidacy request, CancellationToken cancellationToken)
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

            if (!meeting.IsAttendeeAllowedToVote(attendee))
            {
                return Errors.Meetings.YouHaveNoVotingRights;
            }

            var agendaItem = meeting.GetCurrentAgendaItem();

            if (agendaItem is null)
            {
                return Errors.Meetings.NoActiveAgendaItem;
            }

            if (agendaItem.Voting is null || agendaItem.Voting.State == VotingState.Completed)
            {
                return Errors.Meetings.NoOngoingVotingSession;
            }

            var candidate = agendaItem.Candidates.FirstOrDefault(x => x.Id == request.CandidateId);

            if (candidate is null)
            {
                return Errors.Meetings.CandidateAlreadyProposed;
            }

            agendaItem!.RemoveCandidate(candidate);

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}