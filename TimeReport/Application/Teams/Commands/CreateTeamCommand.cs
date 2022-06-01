using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Teams.Commands;

public record CreateTeamCommand(string Name) : IRequest<TeamDto>
{
    public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, TeamDto>
    {
        private readonly ITimeReportContext context;

        public CreateTeamCommandHandler(ITimeReportContext context)
        {
            this.context = context;
        }

        public async Task<TeamDto> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            var team = await context.Teams.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (team is not null) throw new Exception();

            team = new Domain.Entities.Team(request.Name, null);

            context.Teams.Add(team);

            await context.SaveChangesAsync(cancellationToken);

            return team.ToDto();
        }
    }
}
