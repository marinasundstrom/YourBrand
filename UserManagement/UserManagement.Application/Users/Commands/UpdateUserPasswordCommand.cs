using MediatR;

using Microsoft.AspNetCore.Identity;

using YourBrand.UserManagement.Domain.Entities;

namespace YourBrand.UserManagement.Application.Users.Commands;

public record UpdateUserPasswordCommand(string UserId, string CurrentPassword, string NewPassword) : IRequest
{
    public class UpdateUserPasswordCommandHandler : IRequestHandler<UpdateUserPasswordCommand>
    {
        private readonly UserManager<User> _userManager;

        public UpdateUserPasswordCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

        }
    }
}