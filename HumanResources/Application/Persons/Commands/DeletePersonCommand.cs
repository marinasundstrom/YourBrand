using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.HumanResources.Domain.Entities;
using YourBrand.HumanResources.Domain.Exceptions;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Application.Persons.Commands;

public record DeletePersonCommand(string PersonId) : IRequest
{
    public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentPersonService;
        private readonly IEventPublisher _eventPublisher;

        public DeletePersonCommandHandler(IApplicationDbContext context, ICurrentUserService currentPersonService, IEventPublisher eventPublisher)
        {
            _context = context;
            _currentPersonService = currentPersonService;
            _eventPublisher = eventPublisher;
        }

        public async Task<Unit> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
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

            await _eventPublisher.PublishEvent(new PersonDeleted(person.Id, _currentPersonService.UserId));

            return Unit.Value;
        }
    }
}