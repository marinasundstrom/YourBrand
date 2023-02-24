using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Teams
.Commands;

public record AddTeamMemberCommand(string Id, string UserId) : IRequest
{
    public class AddTeamMemberCommandHandler : IRequestHandler<AddTeamMemberCommand>
    {
        private readonly ITimeReportContext context;

        public AddTeamMemberCommandHandler(ITimeReportContext context)
        {
            this.context = context;
        }

        public async Task Handle(AddTeamMemberCommand request, CancellationToken cancellationToken)
        {
            var team = await context.Teams
                .Include(x => x.Memberships)
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (team is null) throw new Exception();

            var user = await context.Users
                .FirstOrDefaultAsync(i => i.Id == request.UserId, cancellationToken);

            if (user is null) throw new Exception();

            team.AddMember(user);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}
