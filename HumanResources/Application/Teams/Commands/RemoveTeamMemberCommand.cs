using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Domain.Exceptions;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Application.Teams.Commands;

public record RemoveTeamMemberCommand(string TeamId, string PersonId) : IRequest
{
    public class Handler : IRequestHandler<RemoveTeamMemberCommand>
    {
        private readonly ICurrentUserService _currentPersonService;
        private readonly IApplicationDbContext _context;
        private readonly IEventPublisher _eventPublisher;

        public Handler(ICurrentUserService currentPersonService, IApplicationDbContext context, IEventPublisher eventPublisher)
        {
            _currentPersonService = currentPersonService;
            _context = context;
            _eventPublisher = eventPublisher;
        }

        public async Task<Unit> Handle(RemoveTeamMemberCommand request, CancellationToken cancellationToken)
        {
            var team = await _context.Teams
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.TeamId, cancellationToken);

            if (team is null)
            {
                throw new PersonNotFoundException(request.TeamId);
            }

            var person = await _context.Persons
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.PersonId, cancellationToken);

            if (person is null)
            {
                throw new PersonNotFoundException(request.PersonId);
            }

            team.RemoveMember(person);

            await _context.SaveChangesAsync(cancellationToken);

            await _eventPublisher.PublishEvent(new Contracts.TeamMemberRemoved(team.Id, person.Id));

            return Unit.Value;
        }
    }
}