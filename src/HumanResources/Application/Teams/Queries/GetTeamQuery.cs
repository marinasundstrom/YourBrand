
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;

namespace YourBrand.HumanResources.Application.Teams.Queries;

public record GetTeamQuery(string TeamId) : IRequest<TeamDto>
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<GetTeamQuery, TeamDto>
    {
        public async Task<TeamDto> Handle(GetTeamQuery request, CancellationToken cancellationToken)
        {
            var team = await context.Teams
                .OrderBy(x => x.Id)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.TeamId, cancellationToken);

            if (team is null)
            {
                return null!;
            }

            return team.ToDto();
        }
    }
}