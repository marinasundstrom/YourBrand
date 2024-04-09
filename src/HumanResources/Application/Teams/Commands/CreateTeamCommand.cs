using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Domain.Entities;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Application.Teams.Commands;

public record CreateTeamCommand(string Name, string Description, string OrganizationId) : IRequest<TeamDto>
{
    public class Handler(IApplicationDbContext context, IUserContext currentPersonService, IEventPublisher eventPublisher) : IRequestHandler<CreateTeamCommand, TeamDto>
    {
        public async Task<TeamDto> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            var team = new Team(request.Name, request.Description);

            team.Organization = await context.Organizations.FirstAsync(/* x => x.Id == request.OrganizationId */);

            context.Teams.Add(team);

            await context.SaveChangesAsync(cancellationToken);

            team = await context.Teams
               .AsNoTracking()
               .AsSplitQuery()
               .Include(x => x.Organization)
               .FirstAsync(x => x.Id == team.Id, cancellationToken);

            await eventPublisher.PublishEvent(new Contracts.TeamCreated(team.Id, team.Organization.Id, team.Name, team.Description!));

            return team.ToDto();
        }
    }
}