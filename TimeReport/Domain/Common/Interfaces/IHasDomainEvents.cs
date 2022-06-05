namespace YourBrand.TimeReport.Domain.Common.Interfaces;

public interface IHasDomainEvents
{
    public List<DomainEvent> DomainEvents { get; set; }
}