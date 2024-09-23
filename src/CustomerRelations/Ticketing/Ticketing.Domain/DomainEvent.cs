using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using MediatR;

namespace YourBrand.Ticketing.Domain;

[NotMapped]
public abstract record DomainEvent : INotification
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime Timestamp { get; } = DateTime.UtcNow;
}