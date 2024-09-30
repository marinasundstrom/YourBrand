using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Ticketing.Application.Features.Teams
.Commands;

public record DeleteTeamCommand(string OrganizationId, string Id) : IRequest
{
    public class DeleteTeamCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteTeamCommand>
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