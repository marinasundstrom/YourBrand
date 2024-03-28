using YourBrand.ApiKeys.Domain.Events;
using YourBrand.ApiKeys.Application.Common.Interfaces;
using YourBrand.ApiKeys.Domain;

namespace YourBrand.ApiKeys.Application.Events;

public class TestEventHandler : IDomainEventHandler<TestEvent>
{
    public Task Handle(TestEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
