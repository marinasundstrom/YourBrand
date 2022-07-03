using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Domain.Entities;

namespace YourBrand.HumanResources.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Role> Roles { get; }

    DbSet<Organization> Organizations { get; }

    DbSet<Person> Persons { get; }

    DbSet<Team> Teams { get; }

    DbSet<TeamMembership> TeamMemberships { get; }

    DbSet<Department> Departments { get; }

    DbSet<BankAccount> BankAccounts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}