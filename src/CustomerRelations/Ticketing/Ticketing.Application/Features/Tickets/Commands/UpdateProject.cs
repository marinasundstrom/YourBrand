using FluentValidation;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;

namespace YourBrand.Ticketing.Application.Tickets.Commands;

public sealed record UpdateProject(string OrganizationId, int Id, int Project) : IRequest<Result>
{
    public sealed class Validator : AbstractValidator<UpdateProject>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public sealed class Handler(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, IApplicationDbContext context, IUserContext userContext) : IRequestHandler<UpdateProject, Result>
    {
        public async Task<Result> Handle(UpdateProject request, CancellationToken cancellationToken)
        {
            var ticket = await ticketRepository.FindByIdAsync(request.Id, cancellationToken);

            if (ticket is null)
            {
                return Result.Failure(Errors.Tickets.TicketNotFound);
            }

            var project = await context.Projects.FirstAsync(x => x.Id == request.Project, cancellationToken);
            ticket.UpdateProject(project.Id!);

            var participant = ticket.Participants.FirstOrDefault(x => x.UserId == userContext.UserId);

            ticket.LastModifiedBy = participant;
            ticket.LastModified = DateTimeOffset.UtcNow;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}