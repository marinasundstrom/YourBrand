using FluentValidation;

using MediatR;

using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Users;

public record GetUserInfo() : IRequest<Result<UserInfoDto>>
{
    public class Validator : AbstractValidator<GetUserInfo>
    {
        public Validator()
        {
        }
    }

    public class Handler(IUserRepository userRepository, ICurrentUserService currentUserService) : IRequestHandler<GetUserInfo, Result<UserInfoDto>>
    {
        public async Task<Result<UserInfoDto>> Handle(GetUserInfo request, CancellationToken cancellationToken)
        {
            var user = await userRepository.FindByIdAsync(currentUserService.UserId!, cancellationToken);

            if (user is null)
            {
                return Result.Failure<UserInfoDto>(Errors.Users.UserNotFound);
            }

            return Result.Success(user.ToDto2());
        }
    }
}