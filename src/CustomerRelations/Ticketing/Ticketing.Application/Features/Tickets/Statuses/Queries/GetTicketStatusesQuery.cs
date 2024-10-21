using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Ticketing.Application.Features.Tickets.Dtos;
using YourBrand.Ticketing.Models;

namespace YourBrand.Ticketing.Application.Features.Tickets.Statuses.Queries;

public record GetTicketStatusesQuery(string OrganizationId, string? SearchTerm, int Page = 0, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<TicketStatusDto>>
{
    sealed class GetTicketStatusesQueryHandler(
        IApplicationDbContext context,
        IUserContext userContext) : IRequestHandler<GetTicketStatusesQuery, PagedResult<TicketStatusDto>>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly IUserContext userContext = userContext;

        public async Task<PagedResult<TicketStatusDto>> Handle(GetTicketStatusesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<TicketStatus> result = _context
                    .TicketStatuses
                    .InOrganization(request.OrganizationId)
                    .OrderBy(o => o.Created)
                    .AsNoTracking()
                    .AsQueryable();

            if (request.SearchTerm is not null)
            {
                result = result.Where(o => o.Name.ToLower().Contains(request.SearchTerm.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                result = result.OrderBy(x => x.Id);
            }

            var items = await result
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new PagedResult<TicketStatusDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}