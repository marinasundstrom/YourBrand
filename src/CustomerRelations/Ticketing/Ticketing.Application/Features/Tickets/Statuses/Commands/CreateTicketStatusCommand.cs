using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Ticketing.Application.Features.Tickets.Dtos;

namespace YourBrand.Ticketing.Application.Features.Tickets.Statuses.Commands;

public record CreateTicketStatusCommand(string OrganizationId, string Name, string Handle, string? Description) : IRequest<TicketStatusDto>
{
    public class CreateTicketStatusCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateTicketStatusCommand, TicketStatusDto>
    {
        private readonly IApplicationDbContext context = context;

        public async Task<TicketStatusDto> Handle(CreateTicketStatusCommand request, CancellationToken cancellationToken)
        {
            var ticketStatus = await context.TicketStatuses.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (ticketStatus is not null) throw new Exception();

            int ticketStatusNo = 1;

            try
            {
                ticketStatusNo = await context.TicketStatuses
                    .InOrganization(request.OrganizationId)
                    .MaxAsync(x => x.Id, cancellationToken) + 1;
            }
            catch { }

            ticketStatus = new Domain.Entities.TicketStatus(ticketStatusNo, request.Name, request.Handle, request.Description);
            ticketStatus.OrganizationId = request.OrganizationId;

            context.TicketStatuses.Add(ticketStatus);

            await context.SaveChangesAsync(cancellationToken);

            return ticketStatus.ToDto();
        }
    }
}