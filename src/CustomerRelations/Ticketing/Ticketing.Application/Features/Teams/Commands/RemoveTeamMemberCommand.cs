using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Ticketing.Application.Features.Teams
.Commands;

public record RemoveTeamMemberCommand(string OrganizationId, string Id, string UserId) : IRequest
{
    public class RemoveTeamMemberCommandHandler(IApplicationDbContext context) : IRequestHandler<RemoveTeamMemberCommand>
    {
        public async Task Handle(RemoveTeamMemberCommand request, CancellationToken cancellationToken)
        {
            var team = await context.Teams
                .InOrganization(request.OrganizationId)
                .Include(x => x.Memberships)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (team is null) throw new Exception();

            var user = await context.Users
                .FirstOrDefaultAsync(i => i.Id == request.UserId, cancellationToken);

            if (user is null) throw new Exception();

            team.RemoveMember(user);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}