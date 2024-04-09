using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.HumanResources.Domain.Exceptions;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Application.Persons.Commands;

public record DeletePersonCommand(string PersonId) : IRequest
{
    public class DeletePersonCommandHandler(IApplicationDbContext context, IUserContext currentPersonService, IEventPublisher eventPublisher) : IRequestHandler<DeletePersonCommand>
    {
        public async Task Handle(DeletePersonCommand request, CancellationToken cancellationToken)
        {
            var person = await context.Persons
                .Include(p => p.Roles)
                .FirstOrDefaultAsync(p => p.Id == request.PersonId);

            if (person is null)
            {
                throw new PersonNotFoundException(request.PersonId);
            }

            context.Persons.Remove(person);

            await context.SaveChangesAsync(cancellationToken);

            await eventPublisher.PublishEvent(new PersonDeleted(person.Id));

        }
    }
}