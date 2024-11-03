using MediatR;

using YourBrand.Accounting.Domain.Common;
using YourBrand.Domain;

namespace YourBrand.Accounting.Application.Common.Models;

public class DomainEventNotification<TDomainEvent>(TDomainEvent domainEvent) : INotification where TDomainEvent : DomainEvent
{
    public TDomainEvent DomainEvent { get; } = domainEvent;
}