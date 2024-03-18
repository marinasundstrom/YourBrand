
using System.Security.Claims;

using IdentityModel;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.UserManagement.Application.Common.Interfaces;
using YourBrand.UserManagement.Contracts;
using YourBrand.UserManagement.Domain.Entities;
using YourBrand.Identity;

namespace YourBrand.UserManagement.Application.Users.Commands;

public record CreateUserCommand(string OrganizationId, string FirstName, string LastName, string? DisplayName, string? Role, string Email) : IRequest<UserDto>
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEventPublisher _eventPublisher;

        public CreateUserCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IEventPublisher eventPublisher)
        {
            _context = context;
            _currentUserService = currentUserService;
            _eventPublisher = eventPublisher;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User(request.FirstName, request.LastName, request.DisplayName, request.Email);

            var organization = await _context.Organizations.FirstOrDefaultAsync(x => x.Id == request.OrganizationId, cancellationToken);

            if (organization is not null)
            {
                throw new Exception();
            }
                
            user.Organization = organization;

            var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == request.Role, cancellationToken);

            if(role is not null) 
            {
                user.AddToRole(role);
            }
            
            _context.Users.Add(user);

            await _context.SaveChangesAsync(cancellationToken);

            user = await _context.Users
               .Include(u => u.Roles)
               .Include(u => u.Organization)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstAsync(x => x.Id == user.Id, cancellationToken);

            await _eventPublisher.PublishEvent(new UserCreated(user.Id, user.Organization!.Id, _currentUserService.UserId));

            return user.ToDto();
        }
    }
}