using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Ticketing.Models;

namespace YourBrand.Ticketing.Application.Features.Tickets.Queries;

public record GetTicketEvents(string OrganizationId, int TicketId, int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<TicketEventDto>>
{
    public class Handler(IApplicationDbContext context, IDtoComposer dtoComposer) : IRequestHandler<GetTicketEvents, PagedResult<TicketEventDto>>
    {
        public async Task<PagedResult<TicketEventDto>> Handle(GetTicketEvents request, CancellationToken cancellationToken)
        {
            var ticketId = (YourBrand.Ticketing.Domain.ValueObjects.TicketId)request.TicketId;

            var ticket = await context.Tickets
            /*
                .Include(i => i.Status)
                .Include(i => i.Type)
                .Include(i => i.Assignee)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy) */
                .Include(i => i.Participants)
                .FirstOrDefaultAsync(x => x.Id == ticketId, cancellationToken);

            var query = context.TicketEvents
                .Where(x => x.OrganizationId == request.OrganizationId)
                .Where(x => x.TicketId == ticketId)
                .AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                query = query.OrderByDescending(x => x.OccurredAt);
            }

            var events = await query
                .AsSplitQuery()
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync(cancellationToken);

            return new PagedResult<TicketEventDto>(
                await dtoComposer.ComposeTicketEventDtos(ticket!, events.ToArray(), cancellationToken)
            , totalCount);
        }
    }
}