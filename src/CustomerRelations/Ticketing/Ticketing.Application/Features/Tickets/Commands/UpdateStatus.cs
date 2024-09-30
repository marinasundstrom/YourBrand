using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Identity;

namespace YourBrand.Ticketing.Application.Tickets.Commands;

public sealed record UpdateStatus(string OrganizationId, int Id, int Status) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<UpdateStatus>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, IApplicationDbContext context, IUserContext userContext) : IRequestHandler<UpdateStatus, Result>
    {
        public async Task<Result> Handle(UpdateStatus request, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.FindByIdAsync(request.Id, cancellationToken);

            if (ticket is null)
            {
                return Result.Failure(Errors.Tickets.TicketNotFound);
            }

            var status = await context.TicketStatuses.FirstAsync(x => x.Id == request.Status, cancellationToken);
            ticket.UpdateStatus(status!);

            var participant = ticket.Participants.FirstOrDefault(x => x.UserId == userContext.UserId);

            ticket.LastModifiedBy = participant;
            ticket.LastModified = DateTimeOffset.UtcNow;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}