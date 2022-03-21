
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourCompany.Application.Common.Interfaces;
using YourCompany.Application.Common.Models;

namespace YourCompany.Application.Users.Queries;

public class GetUsersQuery : IRequest<Results<UserDto>>
{
    public GetUsersQuery(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
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

    public Application.Common.Models.SortDirection? SortDirection { get; }

    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Results<UserDto>>
    { 
        readonly ICatalogContext _context;

        public GetUsersQueryHandler(ICatalogContext context)
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
                    || p.SSN.ToLower().Contains(request.SearchString.ToLower())
                    || p.Email.ToLower().Contains(request.SearchString.ToLower()));
            }

            query = query
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize);

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? YourCompany.Application.SortDirection.Descending : YourCompany.Application.SortDirection.Ascending);
            }

            var users = await query.ToListAsync(cancellationToken);

            var dtos = users.Select(user => new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.SSN, user.Email, user.Created, user.Deleted));

            return new Results<UserDto>(dtos, totalItems);
        }
    }
}