using FluentValidation;

using MediatR;

namespace YourBrand.Ticketing.Application.Features.Tickets.Commands;

public sealed record UpdateAssignee(int Id, string? UserId) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<UpdateAssignee>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler(ITicketRepository ticketRepository, IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateAssignee, Result>
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

            ticket.UpdateAssigneeId(request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}