using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YourBrand.Ticketing.Application.Features.Tickets.Dtos;

namespace YourBrand.Ticketing.Application.Tickets.Commands;

public sealed record UpdateStatus(int Id, int Status) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<UpdateStatus>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<UpdateStatus, Result>
    {
        private readonly ITicketRepository ticketRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IApplicationDbContext context;

        public Handler(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, IApplicationDbContext context)
        {
            this.ticketRepository = ticketRepository;
            this.unitOfWork = unitOfWork;
            this.context = context;
        }

        public async Task<Result> Handle(UpdateStatus request, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.FindByIdAsync(request.Id, cancellationToken);

            if (ticket is null)
            {
                return Result.Failure(Errors.Tickets.TicketNotFound);
            }

            var status = await context.TicketStatuses.FirstAsync(x => x.Id == request.Status, cancellationToken);
            ticket.UpdateStatus(status!);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
