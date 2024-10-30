namespace YourBrand.Sales.Domain.Entities;

public interface ISubscriptionParameters
{
    TimeSpan? CancellationFinalizationPeriod { get; }
    RenewalOption RenewalOption { get; }
    public SubscriptionSchedule Schedule { get; }
}