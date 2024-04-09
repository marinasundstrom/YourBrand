using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityManagement.Application.Common.Interfaces;

namespace YourBrand.IdentityManagement.Application.Users.Commands;

public record UpdateUserRoleCommand(string UserId, string Role) : IRequest
{
    public class UpdateUserRoleCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateUserRoleCommand>
    {
        public async Task Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .Include(p => p.Roles)
                .FirstOrDefaultAsync(p => p.Id == request.UserId);

            var role = await context.Roles
                .FirstOrDefaultAsync(p => p.Name == request.Role);

            if (role is null)
            {
                throw new Exception();
            }

            user.AddToRole(role);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}