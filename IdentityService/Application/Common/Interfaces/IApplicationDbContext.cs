using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Role> Roles { get; }

    DbSet<Person> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}