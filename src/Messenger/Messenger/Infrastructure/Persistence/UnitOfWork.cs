using YourBrand.Messenger.Domain;

namespace YourBrand.Messenger.Infrastructure.Persistence;

sealed class UnitOfWork(MessengerContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => context.SaveChangesAsync(cancellationToken);
}