using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Ticketing.Application.Features.Tickets.Statuses.Commands;

public record DeleteTicketStatusCommand(string OrganizationId, int Id) : IRequest
{
    public class DeleteTicketStatusCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteTicketStatusCommand>
    {
        private readonly IApplicationDbContext context = context;

        public async Task Handle(DeleteTicketStatusCommand request, CancellationToken cancellationToken)
        {
            var ticketStatus = await context.TicketStatuses
                    .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (ticketStatus is null) throw new Exception();

            context.TicketStatuses.Remove(ticketStatus);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}