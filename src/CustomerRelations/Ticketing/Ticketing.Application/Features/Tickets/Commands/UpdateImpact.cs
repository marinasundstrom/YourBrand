using FluentValidation;

using MediatR;
using YourBrand.Identity;
using YourBrand.Ticketing.Domain.Enums;

namespace YourBrand.Ticketing.Application.Features.Tickets.Commands;

public sealed record UpdateImpact(string OrganizationId, int Id, TicketImpact Impact) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<UpdateImpact>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<UpdateImpact, Result>
    {
        public async Task<Result> Handle(UpdateImpact request, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.FindByIdAsync(request.Id, cancellationToken);

            if (ticket is null)
            {
                return Result.Failure(Errors.Tickets.TicketNotFound);
            }

            ticket.UpdateImpact(request.Impact);

            var participant = ticket.Participants.FirstOrDefault(x => x.UserId == userContext.UserId);

            ticket.LastModifiedBy = participant;
            ticket.LastModified = DateTimeOffset.UtcNow;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}