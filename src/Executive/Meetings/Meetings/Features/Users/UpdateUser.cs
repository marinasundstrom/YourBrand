using FluentValidation;

using MediatR;

using YourBrand.Meetings.Features;

namespace YourBrand.Meetings.Features.Users;

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
                return Errors.Users.UserNotFound;
            }

            user.Name = request.Name;
            user.Email = request.Name;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return user.ToDto2();
        }
    }
}