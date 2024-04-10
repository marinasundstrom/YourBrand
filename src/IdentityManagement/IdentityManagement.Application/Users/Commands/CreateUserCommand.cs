using System.Security.Claims;

using IdentityModel;

using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.IdentityManagement.Domain.Entities;
using YourBrand.Tenancy;

namespace YourBrand.IdentityManagement.Application.Users.Commands;

public record CreateUserCommand(string OrganizationId, string FirstName, string LastName, string? DisplayName, string? Role, string Email, string Password) : IRequest<UserDto>
{
    public class CreateUserCommandHandler(IApplicationDbContext context, UserManager<User> userManager, ITenantContext tenantContext, IEventPublisher eventPublisher) : IRequestHandler<CreateUserCommand, UserDto>
    {
        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var organization = await context.Organizations.FirstOrDefaultAsync(x => x.Id == request.OrganizationId, cancellationToken);

            if (organization is null)
            {
                throw new Exception();
            }

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.DisplayName ?? $"{request.FirstName} {request.LastName}", 
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true,
                TenantId = tenantContext.TenantId.GetValueOrDefault()
            };

            organization.AddUser(user);

            var result = userManager.CreateAsync(user, request.Password).Result;

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = await userManager.AddToRoleAsync(user, "Administrator");

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = userManager.AddClaimsAsync(user, [
                        new Claim(JwtClaimTypes.Name, user.DisplayName!),
                            new Claim(JwtClaimTypes.GivenName, user.FirstName),
                            new Claim(JwtClaimTypes.FamilyName, user.LastName),
                            new Claim("tenant_id", tenantContext.TenantId.GetValueOrDefault())
                    ]).Result;

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            /*
            var user = new User(request.FirstName, request.LastName, request.DisplayName, request.Email);

            var organization = await context.Organizations.FirstOrDefaultAsync(x => x.Id == request.OrganizationId, cancellationToken);

            if (organization is not null)
            {
                throw new Exception();
            }

            //user.Organization = organization;

            var role = await context.Roles.FirstOrDefaultAsync(x => x.Name == request.Role, cancellationToken);

            if (role is not null)
            {
                user.AddToRole(role);
            }

            context.Users.Add(user);

            await context.SaveChangesAsync(cancellationToken);

            */

            user = await context.Users
               .Include(u => u.Tenant)
               .Include(u => u.Roles)
               .Include(u => u.Organizations)
               .AsNoTracking()
               .AsSplitQuery()
               .FirstAsync(x => x.Id == user.Id, cancellationToken);

            await eventPublisher.PublishEvent(new UserCreated(user.Id, user.Tenant!.Id, user.Organization!.Id));

            return user.ToDto();
        }
    }
}