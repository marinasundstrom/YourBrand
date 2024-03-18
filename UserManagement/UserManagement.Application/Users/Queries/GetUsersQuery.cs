
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.UserManagement.Application.Common.Interfaces;
using YourBrand.UserManagement.Application.Common.Models;
using YourBrand.UserManagement.Domain.Entities;

namespace YourBrand.UserManagement.Application.Users.Queries;

public record GetUsersQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, UserManagement.Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<UserDto>>
{
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
                    || p.Email.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == UserManagement.Application.Common.Models.SortDirection.Desc ? UserManagement.Application.SortDirection.Descending : UserManagement.Application.SortDirection.Ascending);
            }

            var users = await query
                .Include(u => u.Roles)
                .Include(u => u.Organization)
                .ToListAsync(cancellationToken);

            var dtos = users.Select(user => user.ToDto());

            return new ItemsResult<UserDto>(dtos, totalItems);
        }
    }
}