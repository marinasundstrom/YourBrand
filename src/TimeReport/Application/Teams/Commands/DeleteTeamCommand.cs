﻿using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Teams
.Commands;

public record DeleteTeamCommand(string OrganizationId, string Id) : IRequest
{
    public class DeleteTeamCommandHandler(ITimeReportContext context) : IRequestHandler<DeleteTeamCommand>
    {
        public async Task Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
        {
            var team = await context.Teams
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (team is null) throw new Exception();

            context.Teams.Remove(team);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}