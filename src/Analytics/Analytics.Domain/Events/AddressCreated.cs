using YourBrand.Domain;

namespace YourBrand.Analytics.Domain.Events;

public record AddressCreated(string AddressId) : DomainEvent;