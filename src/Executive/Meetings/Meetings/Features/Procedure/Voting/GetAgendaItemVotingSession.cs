using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Procedure.Voting;

public sealed record GetAgendaItemVotingSession(string OrganizationId, int Id) : IRequest<Result<VotingSessionDto?>>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<GetAgendaItemVotingSession, Result<VotingSessionDto?>>
    {
        public async Task<Result<VotingSessionDto?>> Handle(GetAgendaItemVotingSession request, CancellationToken cancellationToken)
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

            var agendaItem = meeting.GetCurrentAgendaItem();

            if (agendaItem is null)
            {
                return Errors.Meetings.NoActiveAgendaItem;
            }

            if (agendaItem.VotingSession is null) 
            {
                return Errors.Meetings.NoOngoingVotingSession;
            }

            return agendaItem.VotingSession?.ToDto();
        }
    }
}
