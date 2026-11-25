using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Features.Common.Interfaces;

namespace YourBrand.TimeReport.Features.Teams
.Commands;

public record DeleteTeamCommand(string OrganizationId, string Id) : IRequest
{
    public class DeleteTeamCommandHandler(ITimeReportContext context) : IRequestHandler<DeleteTeamCommand>
    {
        public async Task Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
        {
            var team = await context.Teams
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (team is null) throw new Exception();

            context.Teams.Remove(team);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}