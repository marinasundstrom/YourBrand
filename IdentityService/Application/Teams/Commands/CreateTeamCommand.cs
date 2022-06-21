
using System.Security.Claims;

using IdentityModel;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityService.Application.Common.Interfaces;
using YourBrand.IdentityService.Contracts;
using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Application.Teams.Commands;

public record CreateTeamCommand(string Name, string Description) : IRequest<TeamDto>
{
    public class Handler : IRequestHandler<CreateTeamCommand, TeamDto>
    {
        private readonly IApplicationDbContext _context;

        public Handler(UserManager<Person> userManager, IApplicationDbContext context, ICurrentUserService currentUserService, IEventPublisher eventPublisher)
        {
            _context = context;
        }

        public async Task<TeamDto> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            var team = new Team(request.Name, request.Description);

            team = await _context.Teams
               .AsNoTracking()
               .AsSplitQuery()
               .FirstAsync(x => x.Id == team.Id, cancellationToken);

            return team.ToDto();
        }
    }
}