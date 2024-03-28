using FluentValidation;
using MediatR;

namespace YourBrand.Ticketing.Application.Features.Users;

public record GetUserInfo() : IRequest<Result<UserInfoDto>>
{
    public class Validator : AbstractValidator<GetUserInfo>
    {
        public Validator()
        {
        }
    }

    public class Handler : IRequestHandler<GetUserInfo, Result<UserInfoDto>>
    {
        private readonly IUserRepository userRepository;
        private readonly ICurrentUserService currentUserService;

        public Handler(IUserRepository userRepository, ICurrentUserService currentUserService)
        {
            this.userRepository = userRepository;
            this.currentUserService = currentUserService;
        }

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
