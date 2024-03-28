using FluentValidation;
using MediatR;

namespace YourBrand.Ticketing.Application.Features.Tickets.Commands;

public sealed record DeleteTicket(int Id) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<DeleteTicket>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<DeleteTicket, Result>
    {
        private readonly ITicketRepository ticketRepository;
        private readonly IUnitOfWork unitOfWork;

        public Handler(ITicketRepository ticketRepository, IUnitOfWork unitOfWork)
        {
            this.ticketRepository = ticketRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteTicket request, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.FindByIdAsync(request.Id, cancellationToken);

            if (ticket is null)
            {
                return Result.Failure(Errors.Tickets.TicketNotFound);
            }

            ticketRepository.Remove(ticket);

            ticket.AddDomainEvent(new TicketDeleted(ticket.Id, ticket.Subject));

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
