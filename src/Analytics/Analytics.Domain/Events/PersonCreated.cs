using YourBrand.Domain;

namespace YourBrand.Analytics.Domain.Events;

public record ContactCreated(string ContactId) : DomainEvent;