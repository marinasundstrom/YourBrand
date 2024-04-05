using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.IdentityManagement.Domain.Exceptions;

namespace YourBrand.IdentityManagement.Application.Users.Commands;

public record UpdateOrganizationCommand(string UserId, string FirstName, string LastName, string? DisplayName, string Title, string Ssn, string Email, string ReportsTo) : IRequest<UserDto>
{
    public class UpdateUserDetailsCommandHandler : IRequestHandler<UpdateOrganizationCommand, UserDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContext _userContext;
        private readonly IEventPublisher _eventPublisher;

        public UpdateUserDetailsCommandHandler(IApplicationDbContext context, IUserContext userContext, IEventPublisher eventPublisher)
        {
            _context = context;
            _userContext = userContext;
            _eventPublisher = eventPublisher;
        }

        public async Task<UserDto> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .Include(u => u.Organizations)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                throw new UserNotFoundException(request.UserId);
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.DisplayName = request.DisplayName;
            user.Email = request.Email;

            await _context.SaveChangesAsync(cancellationToken);

            await _eventPublisher.PublishEvent(new UserUpdated(user.Id, _userContext.UserId));

            return user.ToDto();
        }
    }
}