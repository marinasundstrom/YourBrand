using System.ComponentModel.DataAnnotations.Schema;

using YourBrand.Domain;

namespace YourBrand.Documents.Domain.Common;

public abstract class Entity<TId> : IEntity<TId>, IEquatable<Entity<TId>>, IHasDomainEvents
    where TId : notnull
{
    private readonly List<DomainEvent> domainEvents = new List<DomainEvent>();

#nullable disable

    protected Entity() { }

#nullable restore

    protected Entity(TId id)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id), "Id cannot be null.");
    }

    public TId Id { get; private set; }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> entity) return false;
        return Id?.Equals(entity.Id) ?? false;
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
        if (other is null) return false;
        return Id?.Equals(other.Id) ?? false;
    }

    public override int GetHashCode()
    {
        return Id?.GetHashCode() ?? 0;
    }

    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents => domainEvents.AsReadOnly();

    public void AddDomainEvent(DomainEvent domainEvent)
    {
        if (domainEvent == null) throw new ArgumentNullException(nameof(domainEvent), "Domain event cannot be null.");
        domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        if (domainEvent == null) throw new ArgumentNullException(nameof(domainEvent), "Domain event cannot be null.");
        domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents() => domainEvents.Clear();

    public override string ToString()
    {
        return $"{GetType().Name} [Id={Id}]";
    }
}