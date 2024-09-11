using FluentValidation;

using MediatR;

using YourBrand.Ticketing.Domain;
using YourBrand.Ticketing.Domain.Repositories;

namespace YourBrand.Ticketing.Application.Features.Users;

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

            return Result.Success(user.ToDto2());
        }
    }
}