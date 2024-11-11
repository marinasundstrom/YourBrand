using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Procedure.Voting;

public sealed record GetVoting(string OrganizationId, int Id) : IRequest<Result<VotingSessionDto?>>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<GetVoting, Result<VotingSessionDto?>>
    {
        public async Task<Result<VotingSessionDto?>> Handle(GetVoting request, CancellationToken cancellationToken)
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

            var agendaItem = meeting.GetCurrentAgendaItem();

            if (agendaItem is null)
            {
                return Errors.Meetings.NoActiveAgendaItem;
            }

            if (agendaItem.Discussion is null)
            {
                return Errors.Meetings.NoOngoingVotingSession;
            }

            return agendaItem.Voting?.ToDto();
        }
    }
}