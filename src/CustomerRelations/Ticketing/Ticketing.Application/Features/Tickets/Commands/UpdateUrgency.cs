using FluentValidation;

using MediatR;
using YourBrand.Identity;
using YourBrand.Ticketing.Domain.Enums;

namespace YourBrand.Ticketing.Application.Features.Tickets.Commands;

public sealed record UpdateUrgency(string OrganizationId, int Id, TicketUrgency Urgency) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<UpdateUrgency>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<UpdateUrgency, Result>
    {
        public async Task<Result> Handle(UpdateUrgency request, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.FindByIdAsync(request.Id, cancellationToken);

            if (ticket is null)
            {
                return Result.Failure(Errors.Tickets.TicketNotFound);
            }

            ticket.UpdateUrgency(request.Urgency);

            var participant = ticket.Participants.FirstOrDefault(x => x.UserId == userContext.UserId);

            ticket.LastModifiedBy = participant;
            ticket.LastModified = DateTimeOffset.UtcNow;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}