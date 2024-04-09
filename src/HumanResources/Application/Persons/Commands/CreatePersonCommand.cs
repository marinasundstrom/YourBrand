using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.HumanResources.Domain.Entities;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Application.Persons.Commands;

public record CreatePersonCommand(string OrganizationId, string FirstName, string LastName, string? DisplayName, string Title, string Role, string Ssn, string Email, string DepartmentId, string? ReportsTo, string Password) : IRequest<PersonDto>
{
    public class CreatePersonCommandHandler(IApplicationDbContext context, IUserContext currentPersonService, IEventPublisher eventPublisher) : IRequestHandler<CreatePersonCommand, PersonDto>
    {
        public async Task<PersonDto> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var organization = await context.Organizations.FirstAsync(); // FirstAsync(o => o.Id == request.OrganizationId, cancellationToken);

            var person = new Person(organization, request.FirstName, request.LastName, request.DisplayName, request.Title, request.Ssn, request.Email);

            var role = await context.Roles.FirstAsync(x => x.Name == request.Role, cancellationToken);

            person.AddToRole(role);

            if (request.ReportsTo != null)
            {
                var manager = await context.Persons.FirstAsync(x => x.Id == request.ReportsTo);
                person.ReportsTo = manager;
            }

            context.Persons.Add(person);

            await context.SaveChangesAsync(cancellationToken);

            person = await context.Persons
               .Include(u => u.Roles)
               .Include(u => u.Organization)
               .Include(u => u.Department)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstAsync(x => x.Id == person.Id, cancellationToken);

            await eventPublisher.PublishEvent(new PersonCreated(person.Id));

            return person.ToDto();
        }
    }
}