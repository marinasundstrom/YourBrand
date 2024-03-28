using YourBrand.Ticketing.Infrastructure.Services;

namespace YourBrand.Ticketing.Infrastructure.Persistence.Repositories.Mocks;

public sealed class MockUnitOfWork : IUnitOfWork
{
    private readonly List<object> items = new List<object>();
    private readonly IDomainEventDispatcher domainEventDispatcher;
    private readonly List<object> newItems = new List<object>();

    public MockUnitOfWork(IDomainEventDispatcher domainEventDispatcher)
    {
        this.domainEventDispatcher = domainEventDispatcher;
    }

    public List<object> Items => items;

    public List<object> NewItems => newItems;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        DomainEvent[] events = GetDomainEvents();

        // Simulating save to database
        newItems.Clear();

        await DispatchEvents(events, cancellationToken);

        foreach (var newItem in newItems.OfType<IHasDomainEvents>())
        {
            newItem.ClearDomainEvents();
        }

        return 0;
    }

    public async Task DispatchEvents(DomainEvent[] events, CancellationToken cancellationToken)
    {
        foreach (var @event in events)
        {
            await domainEventDispatcher.Dispatch(@event, cancellationToken);
        }
    }

    private DomainEvent[] GetDomainEvents()
    {
        return newItems
            .OfType<IHasDomainEvents>()
            .Select(x => x.DomainEvents)
            .SelectMany(x => x)
            .ToArray();
    }
}