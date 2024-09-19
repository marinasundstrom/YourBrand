using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Ticketing.Models;
using YourBrand.Ticketing.Application.Features.Tickets.Dtos;

namespace YourBrand.Ticketing.Application.Features.Tickets.Queries;

public record GetTickets(string OrganizationId, int[]? Status, string? AssigneeId, int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<TicketDto>>
{
    public class Handler(ITicketRepository ticketRepository) : IRequestHandler<GetTickets, PagedResult<TicketDto>>
    {
        public async Task<PagedResult<TicketDto>> Handle(GetTickets request, CancellationToken cancellationToken)
        {
            var query = ticketRepository.GetAll();

            if (request.Status?.Any() ?? false)
            {
                var statuses = request.Status;
                query = query.Where(x => statuses.Any(z => x.StatusId == z));
            }

            /*

            if (request.AssigneeId is not null)
            {
                query = query.Where(x => x.AssigneeId == request.AssigneeId);
            }

            */

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                query = query.OrderByDescending(x => x.Created);
            }

            var tickets = await query
                .Include(i => i.Status)
                .Include(i => i.Type)
                .Include(i => i.Assignee)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .AsSplitQuery()
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync(cancellationToken);

            return new PagedResult<TicketDto>(tickets.Select(x => x.ToDto()), totalCount);
        }
    }
}

public record GetTicketEvents(string OrganizationId, int TicketId, int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<TicketEventDto>>
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<GetTicketEvents, PagedResult<TicketEventDto>>
    {
        public async Task<PagedResult<TicketEventDto>> Handle(GetTicketEvents request, CancellationToken cancellationToken)
        {
            var ticketId = (YourBrand.Ticketing.Domain.ValueObjects.TicketId)request.TicketId;

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

            return new PagedResult<TicketEventDto>(events.Select(x => {
                return System.Text.Json.JsonSerializer.Deserialize<TicketDomainEvent>(x.Data)!.ToDto(x);
            }), totalCount);
        }
    }
}