using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Teams.Commands;

public record UpdateTeamCommand(string Id, string Name, string? Description) : IRequest<TeamDto>
{
    public class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand, TeamDto>
    {
        private readonly ITimeReportContext context;

        public UpdateTeamCommandHandler(ITimeReportContext context)
        {
            this.context = context;
        }

        public async Task<TeamDto> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
        {
            var team = await context.Teams.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (team is null) throw new Exception();

            team.Name = request.Name;
            team.Description = request.Description;

            await context.SaveChangesAsync(cancellationToken);

            return team.ToDto();
        }
    }
}
