using YourBrand.ApiKeys.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.ApiKeys.Application.Common.Interfaces;

public interface IApiKeysContext
{
    DbSet<ApiKey> ApiKeys { get; }

    DbSet<ApiKeyService> ApiKeyServices { get; }

    DbSet<Domain.Entities.Application> Applications { get; }

    DbSet<Resource> Resources { get; }

    DbSet<Service> Services { get; }

    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}