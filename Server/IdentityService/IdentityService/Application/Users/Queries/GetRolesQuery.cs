
using MediatR;

using Microsoft.EntityFrameworkCore;

using Skynet.IdentityService.Application.Common.Interfaces;
using Skynet.IdentityService.Application.Common.Models;
using Skynet.IdentityService.Domain.Entities;

namespace Skynet.IdentityService.Application.Users.Queries;

public class GetRolesQuery : IRequest<ItemsResult<RoleDto>>
{
    public GetRolesQuery(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchString = searchString;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public int Page { get; }

    public int PageSize { get; }

    public string? RoleId { get; }

    public string? SearchString { get; }

    public string? SortBy { get; }

    public Application.Common.Models.SortDirection? SortDirection { get; }

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
                query = query.OrderBy(request.SortBy, request.SortDirection == IdentityService.Application.Common.Models.SortDirection.Desc ? IdentityService.SortDirection.Descending : IdentityService.SortDirection.Ascending);
            }

            var users = await query.ToListAsync(cancellationToken);

            var dtos = users.Select(user => new RoleDto(user.Id, user.Name));

            return new ItemsResult<RoleDto>(dtos, totalItems);
        }
    }
}