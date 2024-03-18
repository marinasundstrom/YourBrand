using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.UserManagement.Application.Common.Interfaces;
using YourBrand.UserManagement.Domain.Entities;
using YourBrand.UserManagement.Domain.Exceptions;

namespace YourBrand.UserManagement.Application.Users.Commands;

public record UpdateUserRoleCommand(string UserId, string Role) : IRequest
{
    public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateUserRoleCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(p => p.Roles)
                .FirstOrDefaultAsync(p => p.Id == request.UserId);

            var role = await _context.Roles
                .FirstOrDefaultAsync(p => p.Name == request.Role);

            if (role is null)
            {
                throw new Exception();
            }

            user.AddToRole(role);

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}