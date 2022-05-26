namespace Transactions.Domain.Common;

public interface IHasDomainEvents
{
    public List<DomainEvent> DomainEvents { get; set; }
}