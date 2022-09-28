using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Domain;

public interface IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}