using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Domain.Exceptions;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Application.Teams.Commands;

public record DeleteTeamCommand(string TeamId) : IRequest
{
    public class Handler(IUserContext currentPersonService, IApplicationDbContext context, IEventPublisher eventPublisher) : IRequestHandler<DeleteTeamCommand>
    {
        public async Task Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
        {
            var team = await context.Teams
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.TeamId, cancellationToken);

            if (team is null)
            {
                throw new PersonNotFoundException(request.TeamId);
            }

            context.Teams.Remove(team);

            await context.SaveChangesAsync(cancellationToken);

            await eventPublisher.PublishEvent(new Contracts.TeamDeleted(team.Id));

        }
    }
}