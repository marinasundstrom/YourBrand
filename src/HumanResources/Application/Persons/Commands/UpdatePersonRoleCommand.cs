using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;

namespace YourBrand.HumanResources.Application.Persons.Commands;

public record UpdatePersonRoleCommand(string PersonId, string Role) : IRequest
{
    public class UpdatePersonRoleCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdatePersonRoleCommand>
    {
        public async Task Handle(UpdatePersonRoleCommand request, CancellationToken cancellationToken)
        {
            var person = await context.Persons
                .Include(p => p.Roles)
                .FirstOrDefaultAsync(p => p.Id == request.PersonId);

            var role = await context.Roles
                .FirstOrDefaultAsync(p => p.Name == request.Role);

            if (role is null)
            {
                throw new Exception();
            }

            person.AddToRole(role);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}