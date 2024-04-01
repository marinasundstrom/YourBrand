using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain.Entities;

namespace YourBrand.Marketing.Domain;

public interface IMarketingContext
{
    DbSet<Contact> Contacts { get; }

    DbSet<Campaign> Campaigns { get; }

    DbSet<Address> Addresses { get; }

    DbSet<Discount> Discounts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}