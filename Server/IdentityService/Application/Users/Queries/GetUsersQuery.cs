
using MediatR;

using Microsoft.EntityFrameworkCore;

using Skynet.IdentityService.Application.Common.Interfaces;
using Skynet.IdentityService.Application.Common.Models;
using Skynet.IdentityService.Domain.Entities;

namespace Skynet.IdentityService.Application.Users.Queries;

public class GetUsersQuery : IRequest<ItemsResult<UserDto>>
{
    public GetUsersQuery(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, IdentityService.Application.Common.Models.SortDirection? sortDirection = null)
    {
        Page = page;
        PageSize = pageSize;
        SearchString = searchString;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public int Page { get; }

    public int PageSize { get; }

    public string? UserId { get; }

    public string? SearchString { get; }

    public string? SortBy { get; }

    public IdentityService.Application.Common.Models.SortDirection? SortDirection { get; }

    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, ItemsResult<UserDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetUsersQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ItemsResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Users
                .OrderBy(p => p.Created)
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.SearchString is not null)
            {
                query = query.Where(p =>
                p.FirstName.ToLower().Contains(request.SearchString.ToLower())
                || p.LastName.ToLower().Contains(request.SearchString.ToLower())
                || ((p.DisplayName ?? "").ToLower().Contains(request.SearchString.ToLower()))
                || p.SSN.ToLower().Contains(request.SearchString.ToLower())
                || p.Email.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == IdentityService.Application.Common.Models.SortDirection.Desc ? IdentityService.SortDirection.Descending : IdentityService.SortDirection.Ascending);
            }

            var users = await query
                .Include(u => u.Department)
                .ToListAsync(cancellationToken);

            var dtos = users.Select(user => new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.SSN, user.Email,
                user.Department == null ? null : new DepartmentDto(user.Department.Id, user.Department.Name),
                    user.Created, user.Deleted));

            return new ItemsResult<UserDto>(dtos, totalItems);
        }
    }
}