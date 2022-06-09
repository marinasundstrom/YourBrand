
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityService.Application.Common.Interfaces;
using YourBrand.IdentityService.Application.Common.Models;
using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Application.Users.Queries;

public record GetRolesQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<RoleDto>>
{
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, ItemsResult<RoleDto>>
    { 
        readonly IApplicationDbContext _context;

        public GetRolesQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Roles
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
                query = query.OrderBy(request.SortBy, request.SortDirection == IdentityService.Application.Common.Models.SortDirection.Desc ? IdentityService.Application.SortDirection.Descending : IdentityService.Application.SortDirection.Ascending);
            }

            var users = await query.ToListAsync(cancellationToken);

            var dtos = users.Select(user => new RoleDto(user.Id, user.Name));

            return new ItemsResult<RoleDto>(dtos, totalItems);
        }
    }
}