using FluentValidation;

using MediatR;
using YourBrand.Identity;
using YourBrand.Ticketing.Domain.Enums;

namespace YourBrand.Ticketing.Application.Features.Tickets.Commands;

public sealed record UpdatePriority(string OrganizationId, int Id, TicketPriority Priority) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<UpdatePriority>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<UpdatePriority, Result>
    {
        public async Task<Result> Handle(UpdatePriority request, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.FindByIdAsync(request.Id, cancellationToken);

            if (ticket is null)
            {
                return Result.Failure(Errors.Tickets.TicketNotFound);
            }

            ticket.UpdatePriority(request.Priority);

            var participant = ticket.Participants.FirstOrDefault(x => x.UserId == userContext.UserId);

            ticket.LastModifiedBy = participant;
            ticket.LastModified = DateTimeOffset.UtcNow;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}