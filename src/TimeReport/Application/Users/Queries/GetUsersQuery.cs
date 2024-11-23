
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Common.Models;

namespace YourBrand.TimeReport.Application.Users.Queries;

public record GetUsersQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, TimeReport.Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Result<ItemsResult<UserDto>>>
{
    public class GetUsersQueryHandler(ITimeReportContext context) : IRequestHandler<GetUsersQuery, Result<ItemsResult<UserDto>>>
    {
        public async Task<Result<ItemsResult<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var query = context.Users
                .OrderBy(p => p.Created)
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

            query = query
                .Include(x => x.Teams)
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize);

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == TimeReport.Application.Common.Models.SortDirection.Desc ? TimeReport.Application.SortDirection.Descending : TimeReport.Application.SortDirection.Ascending);
            }

            var users = await query.ToListAsync(cancellationToken);

            var dtos = users.Select(user => user.ToDto());

            return new ItemsResult<UserDto>(dtos, totalItems);
        }
    }
}