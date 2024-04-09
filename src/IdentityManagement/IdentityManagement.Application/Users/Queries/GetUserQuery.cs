
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityManagement.Application.Common.Interfaces;

namespace YourBrand.IdentityManagement.Application.Users.Queries;

public record GetUserQuery(string UserId) : IRequest<UserDto>
{
    public class GetUserQueryHandler(IApplicationDbContext context) : IRequestHandler<GetUserQuery, UserDto>
    {
        public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .Include(u => u.Tenant)
                .Include(u => u.Roles)
                .Include(u => u.Organizations)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                return null!;
            }

            return user.ToDto();
        }
    }
}