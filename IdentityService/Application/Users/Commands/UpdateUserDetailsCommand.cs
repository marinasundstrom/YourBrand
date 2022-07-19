using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityService.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.IdentityService.Domain.Exceptions;
using YourBrand.IdentityService.Contracts;

namespace YourBrand.IdentityService.Application.Users.Commands;

public record UpdateUserDetailsCommand(string UserId, string FirstName, string LastName, string? DisplayName, string Ssn, string Email) : IRequest<UserDto>
{
    public class UpdateUserDetailsCommandHandler : IRequestHandler<UpdateUserDetailsCommand, UserDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEventPublisher _eventPublisher;

        public UpdateUserDetailsCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IEventPublisher eventPublisher)
        {
            _context = context;
            _currentUserService = currentUserService;
            _eventPublisher = eventPublisher;
        }

        public async Task<UserDto> Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                throw new UserNotFoundException(request.UserId);
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.DisplayName = request.DisplayName;
            user.SSN = request.Ssn;
            user.Email = request.Email;

            await _context.SaveChangesAsync(cancellationToken);

            await _eventPublisher.PublishEvent(new UserUpdated(user.Id, _currentUserService.UserId));

            return user.ToDto();
        }
    }
}
