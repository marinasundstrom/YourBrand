using FluentValidation;

using MediatR;

namespace YourBrand.Ticketing.Application.Features.Tickets.Commands;

public sealed record UpdateText(string OrganizationId, int Id, string Text) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<UpdateText>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();

            RuleFor(x => x.Text).NotEmpty().MaximumLength(60);
        }
    }

    public sealed class Handler(ITicketRepository ticketRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateText, Result>
    {
        public async Task<Result> Handle(UpdateText request, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.FindByIdAsync(request.Id, cancellationToken);

            if (ticket is null)
            {
                return Result.Failure(Errors.Tickets.TicketNotFound);
            }

            ticket.UpdateText(request.Text);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}