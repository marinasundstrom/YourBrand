using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Meetings.Features;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features.Groups.Queries;

public record GetMeetingGroups(string OrganizationId, int Page = 1, int PageSize = 10, string? SearchTerm = null, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<MeetingGroupDto>>
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<GetMeetingGroups, PagedResult<MeetingGroupDto>>
    {
        public async Task<PagedResult<MeetingGroupDto>> Handle(GetMeetingGroups request, CancellationToken cancellationToken)
        {
            var query = context.MeetingGroups
                .InOrganization(request.OrganizationId)
                .AsNoTracking()
                .Include(x => x.Members.OrderBy(x => x.Created))
                .AsQueryable();

            if (request.SearchTerm is not null)
            {
                query = query.Where(x => x.Name.ToLower().Contains(request.SearchTerm.ToLower()));
            }

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                query = query.OrderByDescending(x => x.Created);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query
                .OrderBy(i => i.Id)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .AsSplitQuery()
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync(cancellationToken);

            return new PagedResult<MeetingGroupDto>(users.Select(x => x.ToDto()), totalCount);
        }
    }
}