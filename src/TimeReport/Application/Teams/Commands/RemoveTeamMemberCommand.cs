using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Teams
.Commands;

public record RemoveTeamMemberCommand(string Id, string UserId) : IRequest
{
    public class RemoveTeamMemberCommandHandler(ITimeReportContext context) : IRequestHandler<RemoveTeamMemberCommand>
    {
        public async Task Handle(RemoveTeamMemberCommand request, CancellationToken cancellationToken)
        {
            var team = await context.Teams
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