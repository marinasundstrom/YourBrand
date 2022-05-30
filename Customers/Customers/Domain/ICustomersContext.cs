using YourBrand.Customers.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Customers.Domain;

public interface ICustomersContext
{
    DbSet<Person> Persons { get; }

    DbSet<Address> Addresses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}