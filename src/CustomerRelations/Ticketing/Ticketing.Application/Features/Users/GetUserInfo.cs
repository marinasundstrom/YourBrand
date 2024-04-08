using FluentValidation;

using MediatR;

using YourBrand.Identity;

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
        private readonly IUserContext userContext;

        public Handler(IUserRepository userRepository, IUserContext userContext)
        {
            this.userRepository = userRepository;
            this.userContext = userContext;
        }

        public async Task<Result<UserInfoDto>> Handle(GetUserInfo request, CancellationToken cancellationToken)
        {
            var user = await userRepository.FindByIdAsync(userContext.UserId.GetValueOrDefault(), cancellationToken);

            if (user is null)
            {
                return Result.Failure<UserInfoDto>(Errors.Users.UserNotFound);
            }

            return Result.Success(user.ToDto2());
        }
    }
}