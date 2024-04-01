using Microsoft.EntityFrameworkCore;

using YourBrand.Domain.Entities;

namespace YourBrand.Application.Common.Interfaces;

public interface IAppServiceContext
{
    DbSet<Item> Items { get; }

    DbSet<Module> Modules { get; }

    DbSet<WidgetArea> WidgetAreas { get; }

    DbSet<Widget> Widgets { get; }

    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}