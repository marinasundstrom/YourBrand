using FluentValidation;

using MediatR;

using YourBrand.Identity;

namespace YourBrand.Ticketing.Application.Features.Users;

public record CreateUser(string Name, string Email) : IRequest<Result<UserInfoDto>>
{
    public class Validator : AbstractValidator<CreateUser>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(60);

            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }

    public sealed class Handler(IUserRepository userRepository, IUnitOfWork unitOfWork, IUserContext userContext) : IRequestHandler<CreateUser, Result<UserInfoDto>>
    {
        public async Task<Result<UserInfoDto>> Handle(CreateUser request, CancellationToken cancellationToken)
        {
            string userId = userContext.UserId!;

            userRepository.Add(new User(userId, request.Name, request.Email));

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var user = await userRepository.FindByIdAsync(userId, cancellationToken);

            if (user is null)
            {
                return Result.Failure<UserInfoDto>(Errors.Users.UserNotFound);
            }

            return Result.Success(user.ToDto2());
        }
    }
}