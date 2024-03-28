using YourBrand.Messenger.Domain;

namespace YourBrand.Messenger.Infrastructure.Persistence;

sealed class UnitOfWork : IUnitOfWork
{
    private readonly MessengerContext _context;

    public UnitOfWork(MessengerContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
}