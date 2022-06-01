using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Teams
;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.Teams
.Queries;

public record GetTeamQuery(string Id) : IRequest<TeamDto?>
{
    class GetTeamQueryHandler : IRequestHandler<GetTeamQuery, TeamDto?>
    {
        private readonly ITimeReportContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetTeamQueryHandler(
            ITimeReportContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<TeamDto?> Handle(GetTeamQuery request, CancellationToken cancellationToken)
        {
            var team = await _context
               .Teams
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (team is null)
            {
                return null;
            }

            return team.ToDto();
        }
    }
}
