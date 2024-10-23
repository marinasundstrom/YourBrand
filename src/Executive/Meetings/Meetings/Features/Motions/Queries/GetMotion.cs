using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Meetings.Domain;
using YourBrand.Meetings.Features;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features.Motions.Queries;

public record GetMotion(string OrganizationId, int? MeetingId = null, int Page = 1, int PageSize = 10, string? SearchTerm = null, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<MotionDto>>
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<GetMotion, PagedResult<MotionDto>>
    {
        public async Task<PagedResult<MotionDto>> Handle(GetMotion request, CancellationToken cancellationToken)
        {
            var query = context.Motions
                .InOrganization(request.OrganizationId)
                .AsNoTracking()
                .AsQueryable();

            /*
            if (request.MeetingId is not null)
            {
                query = query.Where(x => x.MeetingId == request.MeetingId);
            }
            */

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

            return new PagedResult<MotionDto>(users.Select(x => x.ToDto()), totalCount);
        }
    }
}