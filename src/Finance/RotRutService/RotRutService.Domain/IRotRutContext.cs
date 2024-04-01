using Microsoft.EntityFrameworkCore;

using YourBrand.RotRutService.Domain.Entities;

namespace YourBrand.RotRutService.Domain;

public interface IRotRutContext
{
    DbSet<RotRutRequest> RotRutRequests { get; set; }

    DbSet<RotRutCase> RotRutCases { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}