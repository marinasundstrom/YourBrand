
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityService.Application.Common.Interfaces;
using YourBrand.IdentityService.Application.Common.Models;
using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Application.Users.Queries;

public record GetUsersQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, IdentityService.Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<UserDto>>
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
                || p.SSN.ToLower().Contains(request.SearchString.ToLower())
                || p.Email.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == IdentityService.Application.Common.Models.SortDirection.Desc ? IdentityService.Application.SortDirection.Descending : IdentityService.Application.SortDirection.Ascending);
            }

            var users = await query
                .Include(u => u.Roles)
                .ToListAsync(cancellationToken);

            var dtos = users.Select(user => user.ToDto());

            return new ItemsResult<UserDto>(dtos, totalItems);
        }
    }
}