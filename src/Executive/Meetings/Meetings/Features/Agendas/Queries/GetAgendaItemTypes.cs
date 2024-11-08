using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Meetings.Domain;
using YourBrand.Meetings.Features;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features.Agendas.Queries;

public record GetAgendaItemTypes(string OrganizationId, int Page = 1, int PageSize = 10, string? SearchTerm = null, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<AgendaItemTypeDto>>
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<GetAgendaItemTypes, PagedResult<AgendaItemTypeDto>>
    {
        public async Task<PagedResult<AgendaItemTypeDto>> Handle(GetAgendaItemTypes request, CancellationToken cancellationToken)
        {
            var query = context.AgendaItemTypes
                //.InOrganization(request.OrganizationId)
                .AsNoTracking()
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
                query = query.OrderByDescending(x => x.Id);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var types = await query
                .OrderBy(i => i.Id)
                //.Include(i => i.CreatedBy)
                //.Include(i => i.LastModifiedBy)
                .AsSplitQuery()
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync(cancellationToken);

            return new PagedResult<AgendaItemTypeDto>(types.Select(x => x.ToDto()), totalCount);
        }
    }
}