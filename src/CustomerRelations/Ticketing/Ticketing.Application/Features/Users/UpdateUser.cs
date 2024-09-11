using FluentValidation;

using MediatR;

using YourBrand.Ticketing.Domain;

namespace YourBrand.Ticketing.Application.Features.Users;

public record UpdateUser(string UserId, string Name, string Email) : IRequest<Result<UserInfoDto>>
{
    public class Validator : AbstractValidator<UpdateUser>
    {
        public Validator()
        {
        }
    }

    public class Handler(IUserRepository userRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateUser, Result<UserInfoDto>>
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