using FluentValidation;

using MediatR;

using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.Features.OrderManagement.Users;

public record DeleteUser(string UserId) : IRequest<Result<DeleteUser>>
{
    public class Validator : AbstractValidator<DeleteUser>
    {
        public Validator()
        {
        }
    }

    public class Handler : IRequestHandler<DeleteUser, Result>
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

        public async Task<Result> Handle(DeleteUser request, CancellationToken cancellationToken)
        {
            var user = await userRepository.FindByIdAsync(request.UserId!, cancellationToken);

            if (user is null)
            {
                return Result.Failure<UserInfoDto>(Errors.Users.UserNotFound);
            }

            userRepository.Remove(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(user.ToDto2());
        }
    }
}
