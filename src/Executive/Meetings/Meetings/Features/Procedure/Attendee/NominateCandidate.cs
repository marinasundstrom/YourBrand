using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Procedure.Attendee;

public sealed record NominateCandidate(string OrganizationId, int Id, string AttendeeId, string? Statement) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext, TimeProvider timeProvider) : IRequestHandler<NominateCandidate, Result>
    {
        public async Task<Result> Handle(NominateCandidate request, CancellationToken cancellationToken)
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
                return Errors.Meetings.NoOngoingVoting;
            }

            var candidateAttendee = meeting.GetAttendeeByUserId(request.AttendeeId);

            if (candidateAttendee is null)
            {
                return Errors.Meetings.NotAnAttendantOfMeeting;
            }
            
            var election = agendaItem!.Election;

            if (election.IsAlreadyCandidate(candidateAttendee.Id))
            {
                return Errors.Meetings.CandidateAlreadyProposed;
            }

            election.NominateCandidate(timeProvider, candidateAttendee.Id, "", request.Statement, "", null);

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}