using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Teams
.Queries;

public record GetTeamQuery(string OrganizationId, string Id) : IRequest<TeamDto?>
{
    sealed class GetTeamQueryHandler(
        ITimeReportContext context,
        IUserContext userContext) : IRequestHandler<GetTeamQuery, TeamDto?>
    {
        public async Task<TeamDto?> Handle(GetTeamQuery request, CancellationToken cancellationToken)
        {
            var team = await context
               .Teams
               .InOrganization(request.OrganizationId)
               .Include(x => x.Memberships)
               .ThenInclude(x => x.User)
               .ThenInclude(x => x.Organizations)
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