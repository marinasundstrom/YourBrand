using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Meetings.Features.Agendas;

namespace YourBrand.Meetings.Features.Procedure.Voting;

public sealed record GetAgendaItemVoting(string OrganizationId, int Id) : IRequest<Result<VotingDto?>>
{
    public sealed class Handler(IApplicationDbContext context, IUserContext userContext) : IRequestHandler<GetAgendaItemVoting, Result<VotingDto?>>
    {
        public async Task<Result<VotingDto?>> Handle(GetAgendaItemVoting request, CancellationToken cancellationToken)
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

            if (agendaItem.Voting is null)
            {
                return Errors.Meetings.NoOngoingVoting;
            }

            return agendaItem.Voting?.ToDto();
        }
    }
}