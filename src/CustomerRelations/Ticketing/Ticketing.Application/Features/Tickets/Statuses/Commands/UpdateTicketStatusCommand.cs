using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Ticketing.Application.Features.Tickets.Dtos;

namespace YourBrand.Ticketing.Application.Features.Tickets.Statuses.Commands;

public record UpdateTicketStatusCommand(string OrganizationId, int Id, string Name, string Handle, string? Description) : IRequest<TicketStatusDto>
{
    public class UpdateTicketStatusCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateTicketStatusCommand, TicketStatusDto>
    {
        private readonly IApplicationDbContext context = context;

        public async Task<TicketStatusDto> Handle(UpdateTicketStatusCommand request, CancellationToken cancellationToken)
        {
            var ticketStatus = await context.TicketStatuses
                    .InOrganization(request.OrganizationId)
                    .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (ticketStatus is null) throw new Exception();

            ticketStatus.Name = request.Name;
            ticketStatus.Handle = request.Handle;
            ticketStatus.Description = request.Description;

            await context.SaveChangesAsync(cancellationToken);

            return ticketStatus.ToDto();
        }
    }
}