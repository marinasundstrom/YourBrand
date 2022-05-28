namespace YourBrand.Payments.Domain.Common;

public interface IHasDomainEvents
{
    public List<DomainEvent> DomainEvents { get; set; }
}