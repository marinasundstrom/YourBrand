using FluentValidation;

using MediatR;

namespace YourBrand.Ticketing.Application.Features.Tickets.Commands;

public sealed record UpdateSubject(string OrganizationId, int Id, string Subject) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<UpdateSubject>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Subject).NotEmpty().MaximumLength(60);
        }
    }

    public sealed class Handler(ITicketRepository ticketRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateSubject, Result>
    {
        public async Task<Result> Handle(UpdateSubject request, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.FindByIdAsync(request.Id, cancellationToken);

            if (ticket is null)
            {
                return Result.Failure(Errors.Tickets.TicketNotFound);
            }

            ticket.UpdateSubject(request.Subject);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}