﻿using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Ticketing.Application.Features.Teams
.Commands;

public record AddTeamMemberCommand(string OrganizationId, string Id, string UserId) : IRequest
{
    public class AddTeamMemberCommandHandler(IApplicationDbContext context) : IRequestHandler<AddTeamMemberCommand>
    {
        public async Task Handle(AddTeamMemberCommand request, CancellationToken cancellationToken)
        {
            var team = await context.Teams
                .InOrganization(request.OrganizationId)
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