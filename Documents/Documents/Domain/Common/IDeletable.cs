namespace YourBrand.Documents.Domain.Common;

public interface IDeletable
{
    DomainEvent GetDeleteEvent();
}