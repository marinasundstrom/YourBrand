using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.HumanResources.Domain.Exceptions;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Application.Persons.Commands;

public record DeletePersonCommand(string PersonId) : IRequest
{
    public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContext _currentPersonService;
        private readonly IEventPublisher _eventPublisher;

        public DeletePersonCommandHandler(IApplicationDbContext context, IUserContext currentPersonService, IEventPublisher eventPublisher)
        {
            _context = context;
            _currentPersonService = currentPersonService;
            _eventPublisher = eventPublisher;
        }

        public async Task Handle(DeletePersonCommand request, CancellationToken cancellationToken)
        {
            var person = await _context.Persons
                .Include(p => p.Roles)
                .FirstOrDefaultAsync(p => p.Id == request.PersonId);

            if (person is null)
            {
                throw new PersonNotFoundException(request.PersonId);
            }

            _context.Persons.Remove(person);

            await _context.SaveChangesAsync(cancellationToken);

            await _eventPublisher.PublishEvent(new PersonDeleted(person.Id));

        }
    }
}