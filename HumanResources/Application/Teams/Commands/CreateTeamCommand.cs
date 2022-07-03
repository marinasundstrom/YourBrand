
using System.Security.Claims;

using IdentityModel;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.HumanResources.Domain.Entities;

namespace YourBrand.HumanResources.Application.Teams.Commands;

public record CreateTeamCommand(string Name, string Description) : IRequest<TeamDto>
{
    public class Handler : IRequestHandler<CreateTeamCommand, TeamDto>
    {
        private readonly IApplicationDbContext _context;

        public Handler(IApplicationDbContext context, ICurrentPersonService currentPersonService, IEventPublisher eventPublisher)
        {
            _context = context;
        }

        public async Task<TeamDto> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            var team = new Team(request.Name, request.Description);

            team.Organization = await _context.Organizations.FirstAsync();

            _context.Teams.Add(team);

            await _context.SaveChangesAsync(cancellationToken);

            team = await _context.Teams
               .AsNoTracking()
               .AsSplitQuery()
               .FirstAsync(x => x.Id == team.Id, cancellationToken);

            return team.ToDto();
        }
    }
}