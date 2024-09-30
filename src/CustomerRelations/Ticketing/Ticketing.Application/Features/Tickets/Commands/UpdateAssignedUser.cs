using FluentValidation;

using MediatR;
using YourBrand.Identity;

namespace YourBrand.Ticketing.Application.Features.Tickets.Commands;

public sealed record UpdateAssignee(string OrganizationId, int Id, string? UserId) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<UpdateAssignee>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler(ITicketRepository ticketRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<UpdateAssignee, Result>
    {
        public async Task<Result> Handle(UpdateAssignee request, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.FindByIdAsync(request.Id, cancellationToken);

            if (ticket is null)
            {
                return Result.Failure(Errors.Tickets.TicketNotFound);
            }

            if (request.UserId is not null)
            {
                var user = await userRepository.FindByIdAsync(request.UserId, cancellationToken);

                if (user is null)
                {
                    return Result.Failure(Errors.Users.UserNotFound);
                }
            }

            var participant = ticket.Participants.FirstOrDefault(x => x.UserId == request.UserId);

            if(participant is null) 
            {
                participant = new TicketParticipant
                {
                    OrganizationId = request.OrganizationId,
                    Name = null,
                    UserId = request.UserId
                };

                ticket.Participants.Add(participant);

                await unitOfWork.SaveChangesAsync(cancellationToken);
            }

            ticket.UpdateAssignee(participant.Id);

            var participant2 = ticket.Participants.FirstOrDefault(x => x.UserId == userContext.UserId);

            ticket.LastModifiedBy = participant2;
            ticket.LastModified = DateTimeOffset.UtcNow;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}