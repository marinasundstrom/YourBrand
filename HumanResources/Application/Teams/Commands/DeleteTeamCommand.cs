using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.HumanResources.Domain.Entities;
using YourBrand.HumanResources.Domain.Exceptions;

namespace YourBrand.HumanResources.Application.Teams.Commands;

public record DeleteTeamCommand(string PersonId) : IRequest
{
    public class Handler : IRequestHandler<DeleteTeamCommand>
    {
        private readonly ICurrentPersonService _currentPersonService;
        private readonly IEventPublisher _eventPublisher;

        public Handler(ICurrentPersonService currentPersonService, IEventPublisher eventPublisher)
        {
            _currentPersonService = currentPersonService;
            _eventPublisher = eventPublisher;
        }

        public async Task<Unit> Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
        {
            /*
            var person = await _personManager.FindByIdAsync(request.PersonId);

            if (person is null)
            {
                throw new PersonNotFoundException(request.PersonId);
            }

            await _personManager.DeleteAsync(person);

            await _eventPublisher.PublishEvent(new PersonDeleted(person.Id, _currentPersonService.PersonId));
            */

            return Unit.Value;
        }
    }
}
