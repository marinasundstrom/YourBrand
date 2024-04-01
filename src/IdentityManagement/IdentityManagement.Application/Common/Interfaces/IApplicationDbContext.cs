using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityManagement.Domain.Entities;

namespace YourBrand.IdentityManagement.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }

    DbSet<Organization> Organizations { get; }

    DbSet<OrganizationUser> OrganizationUsers { get; }

    DbSet<Role> Roles { get; }
    
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}