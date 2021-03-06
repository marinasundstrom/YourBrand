using YourBrand.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace YourBrand.Application.Common.Interfaces;

public interface ICatalogContext
{
    DbSet<Item> Items { get; }

    DbSet<Comment> Comments { get; }

    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}