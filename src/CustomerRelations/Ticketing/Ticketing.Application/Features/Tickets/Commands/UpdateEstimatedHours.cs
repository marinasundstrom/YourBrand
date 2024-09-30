using FluentValidation;

using MediatR;
using YourBrand.Identity;

namespace YourBrand.Ticketing.Application.Features.Tickets.Commands;

public sealed record UpdateEstimatedHours(string OrganizationId, int Id, double? Hours) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<UpdateEstimatedHours>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<UpdateEstimatedHours, Result>
    {
        public async Task<Result> Handle(UpdateEstimatedHours request, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.FindByIdAsync(request.Id, cancellationToken);

            if (ticket is null)
            {
                return Result.Failure(Errors.Tickets.TicketNotFound);
            }

            ticket.UpdateEstimatedHours(request.Hours);

            var participant = ticket.Participants.FirstOrDefault(x => x.UserId == userContext.UserId);

            ticket.LastModifiedBy = participant;
            ticket.LastModified = DateTimeOffset.UtcNow;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}