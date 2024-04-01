using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Teams
.Queries;

public record GetTeamQuery(string Id) : IRequest<TeamDto?>
{
    class GetTeamQueryHandler : IRequestHandler<GetTeamQuery, TeamDto?>
    {
        private readonly ITimeReportContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetTeamQueryHandler(
            ITimeReportContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<TeamDto?> Handle(GetTeamQuery request, CancellationToken cancellationToken)
        {
            var team = await _context
               .Teams
               .Include(x => x.Memberships)
               .ThenInclude(x => x.User)
               .ThenInclude(x => x.Organization)
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