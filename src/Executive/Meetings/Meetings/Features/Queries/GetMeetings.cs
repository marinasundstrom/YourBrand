using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Meetings.Features;
using YourBrand.Meetings.Models;
using YourBrand.Meetings.Domain;
 
namespace YourBrand.Meetings.Features.Queries;

public record GetMeetings(string OrganizationId, int Page = 1, int PageSize = 10, string? SearchTerm = null, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<MeetingDto>>
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<GetMeetings, PagedResult<MeetingDto>>
    {
        public async Task<PagedResult<MeetingDto>> Handle(GetMeetings request, CancellationToken cancellationToken)
        {
            var query = context.Meetings
                .InOrganization(request.OrganizationId)
                .AsNoTracking()
                .Include(x => x.Participants.OrderBy(x => x.Created))
                .AsQueryable();

            if (request.SearchTerm is not null)
            {
                query = query.Where(x => x.Title.ToLower().Contains(request.SearchTerm.ToLower()));
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

            return new PagedResult<MeetingDto>(users.Select(x => x.ToDto()), totalCount);
        }
    }
}