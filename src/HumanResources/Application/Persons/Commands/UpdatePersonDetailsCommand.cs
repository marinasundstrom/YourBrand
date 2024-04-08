using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.HumanResources.Domain.Exceptions;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Application.Persons.Commands;

public record UpdateOrganizationCommand(string PersonId, string FirstName, string LastName, string? DisplayName, string Title, string Ssn, string Email, string ReportsTo) : IRequest<PersonDto>
{
    public class UpdatePersonDetailsCommandHandler : IRequestHandler<UpdateOrganizationCommand, PersonDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContext _currentPersonService;
        private readonly IEventPublisher _eventPublisher;

        public UpdatePersonDetailsCommandHandler(IApplicationDbContext context, IUserContext currentPersonService, IEventPublisher eventPublisher)
        {
            _context = context;
            _currentPersonService = currentPersonService;
            _eventPublisher = eventPublisher;
        }

        public async Task<PersonDto> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var person = await _context.Persons
                .Include(u => u.Roles)
                .Include(u => u.Organization)
                .Include(u => u.Department)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.PersonId, cancellationToken);

            if (person is null)
            {
                throw new PersonNotFoundException(request.PersonId);
            }

            person.FirstName = request.FirstName;
            person.LastName = request.LastName;
            person.DisplayName = request.DisplayName;
            person.Title = request.Title;
            person.SSN = request.Ssn;
            person.Email = request.Email;

            person.ReportsTo = await _context.Persons.FirstAsync(x => x.Id == request.ReportsTo);

            await _context.SaveChangesAsync(cancellationToken);

            await _eventPublisher.PublishEvent(new PersonUpdated(person.Id));

            return person.ToDto();
        }
    }
}