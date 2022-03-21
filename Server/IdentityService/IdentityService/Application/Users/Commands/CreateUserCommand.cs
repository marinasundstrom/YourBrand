
using System.Security.Claims;

using IdentityModel;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourCompany.IdentityService.Application.Common.Interfaces;
using YourCompany.IdentityService.Contracts;
using YourCompany.IdentityService.Domain.Entities;

namespace YourCompany.IdentityService.Application.Users.Commands;

public class CreateUserCommand : IRequest<UserDto>
{
    public CreateUserCommand(string firstName, string lastName, string? displayName, string role, string ssn, string email, string departmentId, string password)
    {
        FirstName = firstName;
        LastName = lastName;
        DisplayName = displayName;
        Role = role;
        SSN = ssn;
        Email = email;
        DepartmentId = departmentId;
        Password = password;
    }

    public string FirstName { get; }

    public string LastName { get; }

    public string? DisplayName { get; }

    public string Role { get; }

    public string SSN { get; }

    public string Email { get; }

    public string DepartmentId { get; }

    public string Password { get; }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEventPublisher _eventPublisher;

        public CreateUserCommandHandler(UserManager<User> userManager, IApplicationDbContext context, ICurrentUserService currentUserService, IEventPublisher eventPublisher)
        {
            _userManager = userManager;
            _context = context;
            _currentUserService = currentUserService;
            _eventPublisher = eventPublisher;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.DisplayName,
                SSN = request.SSN,
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