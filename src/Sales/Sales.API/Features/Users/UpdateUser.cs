using FluentValidation;

using MediatR;

using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.OrderManagement.Repositories;
using YourBrand.Sales.Services;

namespace YourBrand.Sales.Features.OrderManagement.Users;

public record UpdateUser(string UserId, string Name, string Email) : IRequest<Result<UserInfoDto>>
{
    public class Validator : AbstractValidator<UpdateUser>
    {
        public Validator()
        {
        }
    }

    public class Handler(IUserRepository userRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : IRequestHandler<UpdateUser, Result<UserInfoDto>>
    {
        public async Task<Result<UserInfoDto>> Handle(UpdateUser request, CancellationToken cancellationToken)
        {
            var user = await userRepository.FindByIdAsync(request.UserId!, cancellationToken);

            if (user is null)
            {
                return Result.Failure<UserInfoDto>(Errors.Users.UserNotFound);
            }

            user.Name = request.Name;
            user.Email = request.Name;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(user.ToDto2());
        }
    }
}