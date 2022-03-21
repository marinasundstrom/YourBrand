namespace Skynet.IdentityService.Domain.Common.Interfaces;

public interface IHasDomainEvent
{
    public List<DomainEvent> DomainEvents { get; set; }
}