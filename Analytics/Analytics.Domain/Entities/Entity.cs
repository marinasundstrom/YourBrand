using System.ComponentModel.DataAnnotations.Schema;

namespace YourBrand.Analytics.Domain.Entities;

public abstract class Entity<TId> : IEquatable<Entity<TId>>, IHasDomainEvents
    where TId : notnull
{
    private readonly HashSet<DomainEvent> domainEvents = new HashSet<DomainEvent>();

#nullable disable

    protected Entity()
    {

    }

#nullable restore

    protected Entity(TId id)
    {
        Id = id;
    }

    public TId Id { get; protected set; }

    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> entity && Id.Equals(entity.Id);
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !Equals(left, right);
    }

    public bool Equals(Entity<TId>? other)
    {
        return Equals((object?)other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents => domainEvents;

    public void AddDomainEvent(DomainEvent domainEvent) => domainEvents.Add(domainEvent);

    public void RemoveDomainEvent(DomainEvent domainEvent) => domainEvents.Remove(domainEvent);

    public void ClearDomainEvents() => domainEvents.Clear();
}
