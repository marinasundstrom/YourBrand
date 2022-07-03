
using System.Security.Claims;

using IdentityModel;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.HumanResources.Domain.Entities;

namespace YourBrand.HumanResources.Application.Persons.Commands;

public record CreatePersonCommand(string FirstName, string LastName, string? DisplayName, string Title, string Role, string Ssn, string Email, string DepartmentId, string? ReportsTo, string Password) : IRequest<PersonDto>
{
    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, PersonDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentPersonService _currentPersonService;
        private readonly IEventPublisher _eventPublisher;

        public CreatePersonCommandHandler(IApplicationDbContext context, ICurrentPersonService currentPersonService, IEventPublisher eventPublisher)
        {
            _context = context;
            _currentPersonService = currentPersonService;
            _eventPublisher = eventPublisher;
        }

        public async Task<PersonDto> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var person = new Person(request.FirstName, request.LastName, request.DisplayName, request.Title, request.Ssn, request.Email);

            var role = await _context.Roles.FirstAsync(x => x.Name == request.Role);

            person.AddToRole(role);

            if(request.ReportsTo != null)
            {
                var manager = await _context.Persons.FirstAsync(x => x.Id == request.ReportsTo);
                person.ReportsTo = manager;
            }

            _context.Persons.Add(person);

            await _context.SaveChangesAsync(cancellationToken);

            person = await _context.Persons
               .Include(u => u.Roles)
               .Include(u => u.Department)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstAsync(x => x.Id == person.Id, cancellationToken);

            await _eventPublisher.PublishEvent(new PersonCreated(person.Id, _currentPersonService.PersonId));

            return person.ToDto();
        }
    }
}