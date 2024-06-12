using ChatApp.Domain.ValueObjects;
using MediatR;

namespace ChatApp.Domain;

public abstract record DomainEvent : INotification
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime Timestamp { get; } = DateTime.UtcNow;

    public UserId? CurrentUserId { get; set; } 
}