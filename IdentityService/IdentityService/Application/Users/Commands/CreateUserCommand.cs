
using System.Security.Claims;

using IdentityModel;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityService.Application.Common.Interfaces;
using YourBrand.IdentityService.Contracts;
using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Application.Users.Commands;

public record CreateUserCommand(string FirstName, string LastName, string? DisplayName, string Role, string Ssn, string Email, string DepartmentId, string Password) : IRequest<UserDto>
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly UserManager<Person> _userManager;
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEventPublisher _eventPublisher;

        public CreateUserCommandHandler(UserManager<Person> userManager, IApplicationDbContext context, ICurrentUserService currentUserService, IEventPublisher eventPublisher)
        {
            _userManager = userManager;
            _context = context;
            _currentUserService = currentUserService;
            _eventPublisher = eventPublisher;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new Person
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.DisplayName,
                SSN = request.Ssn,
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true
            };

            var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == request.Role);

            if(role is null)
            {
                throw new Exception("Role not found");
            }

            if (request.DepartmentId is not null)
            {
                user.Department = await _context.Departments.FirstAsync(x => x.Id == request.DepartmentId);
            }

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = await _userManager.AddClaimsAsync(user, new Claim[]{
                new Claim(JwtClaimTypes.Name, $"{request.FirstName} {request.LastName}"),
                new Claim(JwtClaimTypes.GivenName, request.FirstName),
                new Claim(JwtClaimTypes.FamilyName, request.LastName)
            });

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = await _userManager.AddToRoleAsync(user, role.Name);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            user = await _context.Users
               .Include(u => u.Roles)
               .Include(u => u.Department)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == user.Id, cancellationToken);

            await _eventPublisher.PublishEvent(new UserCreated(user.Id, _currentUserService.UserId));

            return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.Roles.First().Name, user.SSN, user.Email,
                user.Department == null ? null : new DepartmentDto(user.Department.Id, user.Department.Name),
                    user.Created, user.LastModified);
        }
    }
}