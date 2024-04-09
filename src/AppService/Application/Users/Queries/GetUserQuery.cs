
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;

namespace YourBrand.Application.Users.Queries;

public record GetUserQuery(string UserId) : IRequest<UserDto>
{
    public class GetUserQueryHandler(IAppServiceContext context) : IRequestHandler<GetUserQuery, UserDto>
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

            return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.SSN, user.Email, user.Created, user.LastModified);
        }
    }
}