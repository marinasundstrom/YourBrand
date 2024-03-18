
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.UserManagement.Application.Common.Interfaces;
using YourBrand.UserManagement.Domain.Entities;

namespace YourBrand.UserManagement.Application.Users.Queries;

public record GetUserQuery(string UserId) : IRequest<UserDto>
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
    {
        private readonly IApplicationDbContext _context;

        public GetUserQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .Include(u => u.Organization)
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