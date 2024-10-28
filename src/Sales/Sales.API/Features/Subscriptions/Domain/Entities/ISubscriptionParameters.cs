namespace YourBrand.Sales.Domain.Entities;

public interface ISubscriptionParameters
{
    TimeSpan? CancellationFinalizationPeriod { get; }
    bool AutoRenew { get; }
    public SubscriptionSchedule Schedule { get; }
}