using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Ticketing.Application.Features.Teams.Commands;

public record CreateTeamCommand(string OrganizationId, string Id, string Name, string? Description) : IRequest<TeamDto>
{
    public class CreateTeamCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateTeamCommand, TeamDto>
    {
        public async Task<TeamDto> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            var team = await context.Teams
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (team is not null) throw new Exception();

            team = new Domain.Entities.Team(request.Id, request.Name, request.Description);
            team.Organization = await context.Organizations.FirstAsync(x => x.Id == request.OrganizationId);

            context.Teams.Add(team);

            await context.SaveChangesAsync(cancellationToken);

            return team.ToDto();
        }
    }
}