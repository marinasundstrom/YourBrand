using MediatR;

using Microsoft.AspNetCore.Identity;

using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Application.Users.Commands;

public record UpdateUserRoleCommand(string UserId, string Role) : IRequest
{
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