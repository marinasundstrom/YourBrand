using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Ticketing.Application.Features.Tickets.Dtos;

namespace YourBrand.Ticketing.Application.Features.Tickets.Statuses.Queries;

public record GetTicketStatusQuery(string OrganizationId, int Id) : IRequest<TicketStatusDto?>
{
    sealed class GetTicketStatusQueryHandler(
        IApplicationDbContext context,
        IUserContext userContext) : IRequestHandler<GetTicketStatusQuery, TicketStatusDto?>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly IUserContext userContext = userContext;

        public async Task<TicketStatusDto?> Handle(GetTicketStatusQuery request, CancellationToken cancellationToken)
        {
            var ticketStatus = await _context
               .TicketStatuses
               .InOrganization(request.OrganizationId)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (ticketStatus is null)
            {
                return null;
            }

            return ticketStatus.ToDto();
        }
    }
}