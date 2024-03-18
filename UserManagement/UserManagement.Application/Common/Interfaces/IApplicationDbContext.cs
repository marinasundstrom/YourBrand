using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.UserManagement.Domain.Entities;

namespace YourBrand.UserManagement.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Role> Roles { get; }
    
    DbSet<User> Users { get; }

    DbSet<Organization> Organizations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}