using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Teams
.Queries;

public record GetTeamsQuery(string OrganizationId, int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<TeamDto>>
{
    sealed class GetTeamsQueryHandler(
        ITimeReportContext context,
        IUserContext userContext) : IRequestHandler<GetTeamsQuery, ItemsResult<TeamDto>>
    {
        public async Task<ItemsResult<TeamDto>> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Team> result = context
                    .Teams
                    .OrderBy(o => o.Created)
                    .AsNoTracking()
                    .AsQueryable();

            if (request.SearchString is not null)
            {
                result = result.Where(o => o.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? TimeReport.Application.SortDirection.Descending : TimeReport.Application.SortDirection.Ascending);
            }

            var items = await result
                .Include(x => x.Memberships)
                .ThenInclude(x => x.User)
                .Skip((request.Page) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new ItemsResult<TeamDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}