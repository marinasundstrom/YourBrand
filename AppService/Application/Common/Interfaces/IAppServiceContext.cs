using YourBrand.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace YourBrand.Application.Common.Interfaces;

public interface IAppServiceContext
{
    DbSet<Item> Items { get; }
    
    DbSet<Module> Modules { get; }

    DbSet<Widget> Widgets { get; }

    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}