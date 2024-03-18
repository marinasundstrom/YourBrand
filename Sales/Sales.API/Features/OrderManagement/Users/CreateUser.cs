using FluentValidation;

using MediatR;

using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;
using YourBrand.Sales.API.Features.OrderManagement.Orders;
using YourBrand.Sales.API.Features.OrderManagement.Repositories;

namespace YourBrand.Sales.API.Features.OrderManagement.Users;

public record CreateUser(string Name, string Email, string? UserId = null) : IRequest<Result<UserInfoDto>>
{
    public class Validator : AbstractValidator<CreateUser>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(60);

            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }

    public class Handler : IRequestHandler<CreateUser, Result<UserInfoDto>>
    {
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICurrentUserService currentUserService;

        public Handler(IUserRepository userRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
            this.currentUserService = currentUserService;
        }

        public async Task<Result<UserInfoDto>> Handle(CreateUser request, CancellationToken cancellationToken)
        {
            var user = await userRepository.FindByIdAsync(currentUserService.UserId!, cancellationToken);

            if (user is not null)
            {
                return Result.Success(user.ToDto2());
            }

            string userId = request.UserId ?? currentUserService.UserId!;

            userRepository.Add(new User(userId, request.Name, request.Email));

            await unitOfWork.SaveChangesAsync(cancellationToken);

            user = await userRepository.FindByIdAsync(userId, cancellationToken);

            if (user is null)
            {
                return Result.Failure<UserInfoDto>(Errors.Users.UserNotFound);
            }

            return Result.Success(user.ToDto2());
        }
    }
}