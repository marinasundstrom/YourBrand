
using YourBrand.ApiKeys.Application.Common.Interfaces;
using YourBrand.ApiKeys.Application.Common.Models;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.ApiKeys.Application.Users.Queries;

public record GetUsersQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Results<UserDto>>
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Results<UserDto>>
    { 
        readonly IApiKeysContext _context;

        public GetUsersQueryHandler(IApiKeysContext context)
        {
            _context = context;
        }

        public async Task<Results<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Users
                .OrderBy(p => p.Created)
                .AsNoTracking();
                //.AsSplitQuery();

            if (request.SearchString is not null)
            {
                query = query.Where(p =>
                    p.FirstName.ToLower().Contains(request.SearchString.ToLower())
                    || p.LastName.ToLower().Contains(request.SearchString.ToLower())
                    || ((p.DisplayName ?? "").ToLower().Contains(request.SearchString.ToLower()))
                    || p.Email.ToLower().Contains(request.SearchString.ToLower()));
            }

            query = query
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize);

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Application.SortDirection.Descending : Application.SortDirection.Ascending);
            }

            var users = await query.ToListAsync(cancellationToken);

            var dtos = users.Select(user => new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.Email, user.Created, user.Deleted));

            return new Results<UserDto>(dtos, totalItems);
        }
    }
}