using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Ticketing.Application.Features.Teams
.Queries;

public record GetTeamQuery(string OrganizationId, string Id) : IRequest<TeamDto?>
{
    sealed class GetTeamQueryHandler(
        IApplicationDbContext context,
        IUserContext userContext) : IRequestHandler<GetTeamQuery, TeamDto?>
    {
        public async Task<TeamDto?> Handle(GetTeamQuery request, CancellationToken cancellationToken)
        {
            var team = await context
               .Teams
               .InOrganization(request.OrganizationId)
               .Include(x => x.Memberships)
               .ThenInclude(x => x.User)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (team is null)
            {
                return null;
            }

            return team.ToDto();
        }
    }
}