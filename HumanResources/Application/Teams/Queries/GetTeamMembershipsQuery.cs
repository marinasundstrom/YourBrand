
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Application.Common.Models;
using YourBrand.HumanResources.Domain.Entities;

namespace YourBrand.HumanResources.Application.Teams.Queries;

public record GetTeamMembershipsQuery(string TeamId, int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, HumanResources.Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<TeamMembershipDto>>
{
    public class Handler : IRequestHandler<GetTeamMembershipsQuery, ItemsResult<TeamMembershipDto>>
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<TeamMembershipDto>> Handle(GetTeamMembershipsQuery request, CancellationToken cancellationToken)
        {
            var team = await _context.Teams
                  .FirstOrDefaultAsync(x => x.Id == request.TeamId, cancellationToken);

            if (team is null)
            {
                throw new Exception("Team not found");
            }

            var query = _context.TeamMemberships
                .Where(x => x.TeamId == request.TeamId)
                //.OrderBy(p => p.Created)
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .AsNoTracking()
                .AsSplitQuery();

            /*
            if (request.SearchString is not null)
            {
                query = query.Where(p =>
                    p.Name.ToLower().Contains(request.SearchString.ToLower()));
            }
            */

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == HumanResources.Application.Common.Models.SortDirection.Desc ? HumanResources.Application.SortDirection.Descending : HumanResources.Application.SortDirection.Ascending);
            }

            var teamMemberships = await query
                .Include(x => x.Team)
                .Include(x => x.Person)
                .ThenInclude(x => x.Roles)
                .ToListAsync(cancellationToken);

            return new ItemsResult<TeamMembershipDto>(
                teamMemberships.Select(m => m.ToDto()),
                totalItems);
        }
    }
}