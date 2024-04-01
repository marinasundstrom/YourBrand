using YourBrand.TimeReport.Domain;

namespace YourBrand.TimeReport.Infrastructure.Persistence;

sealed class UnitOfWork : IUnitOfWork
{
    private readonly TimeReportContext _context;

    public UnitOfWork(TimeReportContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
}
