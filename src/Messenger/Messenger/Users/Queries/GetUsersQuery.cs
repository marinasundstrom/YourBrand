
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Application.Common.Models;
using YourBrand.Messenger.Contracts;

namespace YourBrand.Messenger.Application.Users.Queries;

public class GetUsersQuery(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null) : IRequest<Results<UserDto>>
{
    public int Page { get; } = page;

    public int PageSize { get; } = pageSize;

    public string? UserId { get; }

    public string? SearchString { get; } = searchString;

    public string? SortBy { get; } = sortBy;

    public Application.Common.Models.SortDirection? SortDirection { get; } = sortDirection;

    public class GetUsersQueryHandler(IMessengerContext context) : IRequestHandler<GetUsersQuery, Results<UserDto>>
    {
        public async Task<Results<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var query = context.Users
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