using YourBrand.Marketing.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Marketing.Domain;

public interface IMarketingContext
{
    DbSet<Prospect> Prospects { get; }

    DbSet<Campaign> Campaigns { get; }

    DbSet<Address> Addresses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}