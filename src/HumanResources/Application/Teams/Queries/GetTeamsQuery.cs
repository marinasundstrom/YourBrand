
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Application.Common.Models;

namespace YourBrand.HumanResources.Application.Teams.Queries;

public record GetTeamsQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, HumanResources.Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<TeamDto>>
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<GetTeamsQuery, ItemsResult<TeamDto>>
    {
        public async Task<ItemsResult<TeamDto>> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
        {
            var query = context.Teams
                .OrderBy(p => p.Created)
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.SearchString is not null)
            {
                query = query.Where(p =>
                p.Name.ToLower().Contains(request.SearchString.ToLower())
                || p.Description!.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == HumanResources.Application.Common.Models.SortDirection.Desc ? HumanResources.Application.SortDirection.Descending : HumanResources.Application.SortDirection.Ascending);
            }

            var dtos = query.Select(team => team.ToDto());

            return new ItemsResult<TeamDto>(dtos, totalItems);
        }
    }
}