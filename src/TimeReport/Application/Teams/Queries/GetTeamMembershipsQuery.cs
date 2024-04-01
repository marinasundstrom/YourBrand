using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Teams
.Queries;

public record GetTeamMembershipsQuery(string Id, int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<TeamMembershipDto>>
{
    class GetTeamMembershipsQueryHandler : IRequestHandler<GetTeamMembershipsQuery, ItemsResult<TeamMembershipDto>>
    {
        private readonly ITimeReportContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetTeamMembershipsQueryHandler(
            ITimeReportContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<ItemsResult<TeamMembershipDto>> Handle(GetTeamMembershipsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<TeamMembership> result = _context
                    .TeamMemberships
                    .OrderBy(o => o.Created)
                    .Where(t => t.TeamId == request.Id)
                    .AsNoTracking()
                    .AsQueryable();

            if (request.SearchString is not null)
            {
                result = result.Where(o =>
                    o.User.FirstName.ToLower().Contains(request.SearchString.ToLower()) ||
                    o.User.LastName.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? TimeReport.Application.SortDirection.Descending : TimeReport.Application.SortDirection.Ascending);
            }

            var items = await result
                .Include(x => x.User)
                .ThenInclude(t => t.Organization)
                .Skip((request.Page) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new ItemsResult<TeamMembershipDto>(items.Select(cp => cp.ToDto2()), totalCount);
        }
    }
}