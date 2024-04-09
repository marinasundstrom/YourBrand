using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Domain.Exceptions;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Application.Teams.Commands;

public record RemoveTeamMemberCommand(string TeamId, string PersonId) : IRequest
{
    public class Handler(IUserContext currentPersonService, IApplicationDbContext context, IEventPublisher eventPublisher) : IRequestHandler<RemoveTeamMemberCommand>
    {
        public async Task Handle(RemoveTeamMemberCommand request, CancellationToken cancellationToken)
        {
            var team = await context.Teams
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.TeamId, cancellationToken);

            if (team is null)
            {
                throw new PersonNotFoundException(request.TeamId);
            }

            var person = await context.Persons
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.PersonId, cancellationToken);

            if (person is null)
            {
                throw new PersonNotFoundException(request.PersonId);
            }

            team.RemoveMember(person);

            await context.SaveChangesAsync(cancellationToken);

            await eventPublisher.PublishEvent(new Contracts.TeamMemberRemoved(team.Id, person.Id));

        }
    }
}