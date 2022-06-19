using YourBrand.RotRutService.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.RotRutService.Domain;

public interface IRotRutContext
{
    DbSet<RotRutRequest> RotRutRequests { get; set; }

    DbSet<RotRutCase> RotRutCases { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}