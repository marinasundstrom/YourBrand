using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Domain.Entities;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Application.Teams.Commands;

public record CreateTeamCommand(string Name, string Description, string OrganizationId) : IRequest<TeamDto>
{
    public class Handler : IRequestHandler<CreateTeamCommand, TeamDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IEventPublisher _eventPublisher;

        public Handler(IApplicationDbContext context, IUserContext currentPersonService, IEventPublisher eventPublisher)
        {
            _context = context;
            _eventPublisher = eventPublisher;
        }

        public async Task<TeamDto> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            var team = new Team(request.Name, request.Description);

            team.Organization = await _context.Organizations.FirstAsync(/* x => x.Id == request.OrganizationId */);

            _context.Teams.Add(team);

            await _context.SaveChangesAsync(cancellationToken);

            team = await _context.Teams
               .AsNoTracking()
               .AsSplitQuery()
               .Include(x => x.Organization)
               .FirstAsync(x => x.Id == team.Id, cancellationToken);

            await _eventPublisher.PublishEvent(new Contracts.TeamCreated(team.Id, team.Organization.Id, team.Name, team.Description!));

            return team.ToDto();
        }
    }
}