using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Role> Roles { get; }

    DbSet<Organization> Organizations { get; }

    DbSet<Person> Users { get; }

    DbSet<Team> Teams { get; }

    DbSet<TeamMembership> TeamMemberships { get; }

    DbSet<Department> Departments { get; }

    DbSet<BankAccount> BankAccounts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}