
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Application.Common.Models;

namespace YourBrand.IdentityManagement.Application.Users.Queries;

public record GetUserRolesQuery(string UserId, int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, IdentityManagement.Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<RoleDto>>
{
    public class GetUserRolesQueryHandler(IApplicationDbContext context) : IRequestHandler<GetUserRolesQuery, ItemsResult<RoleDto>>
    {
        public async Task<ItemsResult<RoleDto>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                  .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                throw new Exception("User not found");
            }

            var query = context.Roles
                .Where(x => x.Users.Any(x => x.Id == request.UserId))
                //.OrderBy(p => p.Created)
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.SearchString is not null)
            {
                query = query.Where(p =>
                    p.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == IdentityManagement.Application.Common.Models.SortDirection.Desc ? IdentityManagement.Application.SortDirection.Descending : IdentityManagement.Application.SortDirection.Ascending);
            }

            var roles = await query
                .ToListAsync(cancellationToken);

            var dtos = roles.Select(role => new RoleDto(role.Id, role.Name));

            return new ItemsResult<RoleDto>(dtos, totalItems);
        }
    }
}