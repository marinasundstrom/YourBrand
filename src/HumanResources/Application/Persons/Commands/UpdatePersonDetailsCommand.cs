using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.HumanResources.Domain.Exceptions;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Application.Persons.Commands;

public record UpdateOrganizationCommand(string PersonId, string FirstName, string LastName, string? DisplayName, string Title, string Ssn, string Email, string ReportsTo) : IRequest<PersonDto>
{
    public class UpdatePersonDetailsCommandHandler(IApplicationDbContext context, IUserContext currentPersonService, IEventPublisher eventPublisher) : IRequestHandler<UpdateOrganizationCommand, PersonDto>
    {
        public async Task<PersonDto> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var person = await context.Persons
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

            person.ReportsTo = await context.Persons.FirstAsync(x => x.Id == request.ReportsTo);

            await context.SaveChangesAsync(cancellationToken);

            await eventPublisher.PublishEvent(new PersonUpdated(person.Id));

            return person.ToDto();
        }
    }
}