using FluentValidation;

using MediatR;

namespace YourBrand.Meetings.Features.Users;

public record DeleteUser(string UserId) : IRequest<Result<DeleteUser>>
{
    public class Validator : AbstractValidator<DeleteUser>
    {
        public Validator()
        {
        }
    }

    public class Handler(IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteUser, Result>
    {
        public async Task<Result> Handle(DeleteUser request, CancellationToken cancellationToken)
        {
            var user = await userRepository.FindByIdAsync(request.UserId!, cancellationToken);

            if (user is null)
            {
                return Result.Failure<UserInfoDto>(Errors.Users.UserNotFound);
            }

            userRepository.Remove(user);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.SuccessWith(user.ToDto2());
        }
    }
}