using FluentValidation;

using MediatR;

using YourBrand.Sales.API.Features.OrderManagement.Orders;
using YourBrand.Sales.API.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.API.Features.OrderManagement.Users;

public record UpdateUser(string UserId, string Name, string Email) : IRequest<Result<UserInfoDto>>
{
    public class Validator : AbstractValidator<UpdateUser>
    {
        public Validator()
        {
        }
    }

    public class Handler : IRequestHandler<UpdateUser, Result<UserInfoDto>>
    {
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService currentUserService;

        public Handler(IUserRepository userRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            this.userRepository = userRepository;
            _unitOfWork = unitOfWork;
            this.currentUserService = currentUserService;
        }

        public async Task<Result<UserInfoDto>> Handle(UpdateUser request, CancellationToken cancellationToken)
        {
            var user = await userRepository.FindByIdAsync(request.UserId!, cancellationToken);

            if (user is null)
            {
                return Result.Failure<UserInfoDto>(Errors.Users.UserNotFound);
            }

            user.Name = request.Name;
            user.Email = request.Name;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(user.ToDto2());
        }
    }
}