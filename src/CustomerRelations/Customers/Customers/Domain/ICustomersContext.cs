using Microsoft.EntityFrameworkCore;

using YourBrand.Customers.Domain.Entities;

namespace YourBrand.Customers.Domain;

public interface ICustomersContext
{
    DbSet<Customer> Customers { get; }

    DbSet<Person> Persons { get; }

    DbSet<Organization> Organizations { get; }

    DbSet<Address> Addresses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}