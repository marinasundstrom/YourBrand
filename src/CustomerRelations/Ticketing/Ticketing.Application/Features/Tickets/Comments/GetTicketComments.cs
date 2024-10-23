using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Ticketing.Application.Features.Tickets.Dtos;
using YourBrand.Ticketing.Models;

namespace YourBrand.Ticketing.Application.Features.Tickets.Commands;

public record GetTicketComments(string OrganizationId, int TicketId, int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<TicketCommentDto>>
{
    public class Handler(IApplicationDbContext context, IDtoComposer dtoComposer) : IRequestHandler<GetTicketComments, PagedResult<TicketCommentDto>>
    {
        public async Task<PagedResult<TicketCommentDto>> Handle(GetTicketComments request, CancellationToken cancellationToken)
        {
            var ticket = await context.Tickets.FirstOrDefaultAsync(x => x.Id == request.TicketId, cancellationToken);

            if (ticket is null)
                return null!;

            var query = context.TicketComments
                .InOrganization(request.OrganizationId)
                .Where(x => x.TicketId == request.TicketId);

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                query = query.OrderByDescending(x => x.Created);
            }

            var ticketComments = await query
                .AsSplitQuery()
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync(cancellationToken);

            return new PagedResult<TicketCommentDto>(await dtoComposer.ComposeTicketCommentDtos(ticket, ticketComments, cancellationToken), totalCount);
        }
    }
}