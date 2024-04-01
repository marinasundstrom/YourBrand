using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;

namespace YourBrand.HumanResources.Application.Persons.Commands;

public record UpdatePersonRoleCommand(string PersonId, string Role) : IRequest
{
    public class UpdatePersonRoleCommandHandler : IRequestHandler<UpdatePersonRoleCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdatePersonRoleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdatePersonRoleCommand request, CancellationToken cancellationToken)
        {
            var person = await _context.Persons
                .Include(p => p.Roles)
                .FirstOrDefaultAsync(p => p.Id == request.PersonId);

            var role = await _context.Roles
                .FirstOrDefaultAsync(p => p.Name == request.Role);

            if (role is null)
            {
                throw new Exception();
            }

            person.AddToRole(role);

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}