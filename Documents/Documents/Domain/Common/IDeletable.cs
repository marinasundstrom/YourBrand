namespace YourBrand.Documents.Domain.Common;

public interface IDeletable : IHasDomainEvents
{
    DomainEvent GetDeleteEvent();
}