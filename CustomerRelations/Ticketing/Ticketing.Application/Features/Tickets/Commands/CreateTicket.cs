using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YourBrand.Ticketing.Application.Features.Tickets.Dtos;

namespace YourBrand.Ticketing.Application.Features.Tickets.Commands;

public sealed record CreateTicket(string Title, string? Description, int Status, string? AssigneeId, double? EstimatedHours, double? RemainingHours) : IRequest<Result<TicketDto>>
{
    public sealed class Validator : AbstractValidator<CreateTicket>
    {
        public Validator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(60);

            RuleFor(x => x.Description).MaximumLength(240);
        }
    }

    public sealed class Handler : IRequestHandler<CreateTicket, Result<TicketDto>>
    {
        private readonly ITicketRepository ticketRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IApplicationDbContext context;
        private readonly IDomainEventDispatcher domainEventDispatcher;

        public Handler(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, IApplicationDbContext context, IDomainEventDispatcher domainEventDispatcher)
        {
            this.ticketRepository = ticketRepository;
            this.unitOfWork = unitOfWork;
            this.context = context;
            this.domainEventDispatcher = domainEventDispatcher;
        }

        public async Task<Result<TicketDto>> Handle(CreateTicket request, CancellationToken cancellationToken)
        {
            var ticket = new Ticket("", "", "");
            
            ticket.Status = await context.TicketStatuses.FirstAsync(s => s.Id == request.Status, cancellationToken);

            ticket.UpdateEstimatedHours(request.EstimatedHours);
            ticket.UpdateRemainingHours(request.RemainingHours);

            ticketRepository.Add(ticket);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            if (request.AssigneeId is not null)
            {
                //ticket.UpdateAssigneeId(request.AssigneeId);

                await unitOfWork.SaveChangesAsync(cancellationToken);

                ticket.ClearDomainEvents();
            }

            await domainEventDispatcher.Dispatch(new TicketCreated(ticket.Id), cancellationToken);

            ticket = await ticketRepository.GetAll()
                .OrderBy(i => i.Id)
                .Include(i => i.Status)
                .Include(i => i.Type)
                .Include(i => i.Assignee)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .LastAsync(cancellationToken);

            return Result.Success(ticket!.ToDto());
        }
    }
}
