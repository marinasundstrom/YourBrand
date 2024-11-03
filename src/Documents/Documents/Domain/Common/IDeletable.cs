using YourBrand.Domain;

namespace YourBrand.Documents.Domain.Common;

public interface IDeletable
{
    DomainEvent GetDeleteEvent();
}