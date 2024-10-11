using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Meetings.Features.Procedure.Command;

public sealed record CastBallot(string OrganizationId, int Id, string CandidateId) : IRequest<Result>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<CastBallot, Result>
    {
        public async Task<Result> Handle(CastBallot request, CancellationToken cancellationToken)
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

            var participant = meeting.Participants.FirstOrDefault(x => x.UserId == userContext.UserId);

            if (participant is null)
                throw new UnauthorizedAccessException("You are not a participant of this meeting.");

            if (!participant.HasVotingRights)
                throw new UnauthorizedAccessException("You have no voting rights.");

            var agendaItem = meeting.GetCurrentAgendaItem();

            if (agendaItem is null)
            {
                return Errors.Meetings.NoActiveAgendaItem;
            }

            if (agendaItem.VotingSession is null)
                throw new InvalidOperationException("No active voting session.");

            agendaItem.VotingSession!.AddVote(new Vote 
            {
                OrganizationId = request.OrganizationId,
                VoterId = participant.Id,
                SelectedCandidateId = request.CandidateId,
                TimeCast = DateTimeOffset.UtcNow
            });

            context.Meetings.Update(meeting);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
