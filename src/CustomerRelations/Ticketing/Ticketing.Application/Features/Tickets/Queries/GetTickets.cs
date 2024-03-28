using MediatR;
using Microsoft.EntityFrameworkCore;
using YourBrand.Ticketing.Application.Common;
using YourBrand.Ticketing.Application.Features.Tickets.Dtos;
using YourBrand.Ticketing.Domain.Enums;

namespace YourBrand.Ticketing.Application.Features.Tickets.Queries;

public record GetTickets(int[]? Status, string? AssigneeId, int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<ItemsResult<TicketDto>>
{
    public class Handler : IRequestHandler<GetTickets, ItemsResult<TicketDto>>
    {
        private readonly ITicketRepository ticketRepository;

        public Handler(ITicketRepository ticketRepository)
        {
            this.ticketRepository = ticketRepository;
        }

        public async Task<ItemsResult<TicketDto>> Handle(GetTickets request, CancellationToken cancellationToken)
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

            return new ItemsResult<TicketDto>(tickets.Select(x => x.ToDto()), totalCount);
        }
    }
}
