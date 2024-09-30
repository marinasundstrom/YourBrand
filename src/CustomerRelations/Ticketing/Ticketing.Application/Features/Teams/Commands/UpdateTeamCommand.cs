using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Ticketing.Application.Features.Teams.Commands;

public record UpdateTeamCommand(string OrganizationId, string Id, string Name, string? Description) : IRequest<TeamDto>
{
    public class UpdateTeamCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateTeamCommand, TeamDto>
    {
        public async Task<TeamDto> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
        {
            var team = await context.Teams
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (team is null) throw new Exception();

            team.Name = request.Name;
            team.Description = request.Description;

            await context.SaveChangesAsync(cancellationToken);

            return team.ToDto();
        }
    }
}