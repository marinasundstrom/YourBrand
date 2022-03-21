using MediatR;

using Microsoft.AspNetCore.Identity;

using YourCompany.IdentityService.Domain.Entities;

namespace YourCompany.IdentityService.Application.Users.Commands;

public class UpdateUserRoleCommand : IRequest
{
    public UpdateUserRoleCommand(string userId, string role)
    {
        UserId = userId;
        Role = role;
    }

    public string UserId { get; }

    public string Role { get; }

    public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand>
    {
        private readonly UserManager<User> _userManager;

        public UpdateUserRoleCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Unit> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            var result = await _userManager.AddToRoleAsync(user, request.Role);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            return Unit.Value;
        }
    }
}