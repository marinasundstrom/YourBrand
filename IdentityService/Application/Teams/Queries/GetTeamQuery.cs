
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityService.Application.Common.Interfaces;
using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Application.Teams.Queries;

public record GetTeamQuery(string TeamId) : IRequest<TeamDto>
{
    public class Handler : IRequestHandler<GetTeamQuery, TeamDto>
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TeamDto> Handle(GetTeamQuery request, CancellationToken cancellationToken)
        {
            var team = await _context.Teams
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