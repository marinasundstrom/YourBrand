using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Features.Common.Interfaces;

namespace YourBrand.TimeReport.Features.Teams.Commands;

public record UpdateTeamCommand(string OrganizationId, string Id, string Name, string? Description) : IRequest<TeamDto>
{
    public class UpdateTeamCommandHandler(ITimeReportContext context) : IRequestHandler<UpdateTeamCommand, TeamDto>
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