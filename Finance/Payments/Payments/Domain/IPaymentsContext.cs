using Microsoft.EntityFrameworkCore;

using YourBrand.Payments.Domain.Entities;

namespace YourBrand.Payments.Domain;

public interface IPaymentsContext
{
    DbSet<Payment> Payments { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}