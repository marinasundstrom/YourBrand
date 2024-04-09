using MediatR;

using Microsoft.AspNetCore.Identity;

using YourBrand.IdentityManagement.Domain.Entities;

namespace YourBrand.IdentityManagement.Application.Users.Commands;

public record UpdateUserPasswordCommand(string UserId, string CurrentPassword, string NewPassword) : IRequest
{
    public class UpdateUserPasswordCommandHandler(UserManager<User> userManager) : IRequestHandler<UpdateUserPasswordCommand>
    {
        public async Task Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId);

            var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

        }
    }
}