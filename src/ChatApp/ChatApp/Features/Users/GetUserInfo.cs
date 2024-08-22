using FluentValidation;

using MediatR;

using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Domain.Repositories;
using YourBrand.IdentityManagement.Contracts;

namespace YourBrand.ChatApp.Features.Users;

public record GetUserInfo() : IRequest<Result<UserInfoDto>>
{
    public class Validator : AbstractValidator<GetUserInfo>
    {
        public Validator()
        {
        }
    }

    public class Handler(IUserRepository userRepository, IUserContext userContext, ILogger<Handler> logger) : IRequestHandler<GetUserInfo, Result<UserInfoDto>>
    {
        public async Task<Result<UserInfoDto>> Handle(GetUserInfo request, CancellationToken cancellationToken)
        {
            logger.LogError(userContext.UserId);

            var user = await userRepository.FindByIdAsync(userContext.UserId.GetValueOrDefault(), cancellationToken);

            if (user is null)
            {
                return Result.Failure<UserInfoDto>(Errors.Users.UserNotFound);
            }

            return Result.SuccessWith(user.ToDto2());
        }
    }
}