using YourBrand.ApiKeys.Application.Common.Interfaces;
using YourBrand.ApiKeys.Domain.Events;

namespace YourBrand.ApiKeys.Application.Events;

public class TestEventHandler : IDomainEventHandler<TestEvent>
{
    public Task Handle(TestEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}