
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Contracts;

namespace YourBrand.Messenger.Application.Users.Queries;

public class GetUserQuery(string userId) : IRequest<UserDto>
{
    public string UserId { get; } = userId;

    public class GetUserQueryHandler(IMessengerContext context) : IRequestHandler<GetUserQuery, UserDto>
    {
        public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                return null!;
            }

            return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.Email, user.Created, user.LastModified);
        }
    }
}