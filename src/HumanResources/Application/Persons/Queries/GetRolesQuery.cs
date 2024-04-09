
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Application.Common.Models;

namespace YourBrand.HumanResources.Application.Persons.Queries;

public record GetRolesQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<RoleDto>>
{
    public class GetRolesQueryHandler(IApplicationDbContext context) : IRequestHandler<GetRolesQuery, ItemsResult<RoleDto>>
    {
        public async Task<ItemsResult<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var query = context.Roles
                //.OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.SearchString is not null)
            {
                query = query.Where(p =>
                    p.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            query = query
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize);

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == HumanResources.Application.Common.Models.SortDirection.Desc ? HumanResources.Application.SortDirection.Descending : HumanResources.Application.SortDirection.Ascending);
            }

            var persons = await query.ToListAsync(cancellationToken);

            var dtos = persons.Select(person => new RoleDto(person.Id, person.Name));

            return new ItemsResult<RoleDto>(dtos, totalItems);
        }
    }
}