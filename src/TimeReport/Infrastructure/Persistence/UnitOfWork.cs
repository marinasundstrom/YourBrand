using YourBrand.TimeReport.Domain;

namespace YourBrand.TimeReport.Infrastructure.Persistence;

sealed class UnitOfWork(TimeReportContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);
}