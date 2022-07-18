
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Application.Common.Models;
using YourBrand.HumanResources.Domain.Entities;

namespace YourBrand.HumanResources.Application.Teams.Events;

public record TeamCreatedHandler(IApplicationDbContext Context, IEventPublisher EventPublisher) : INotificationHandler<DomainEventNotification<TeamCreated>>
{
    public async Task Handle(DomainEventNotification<TeamCreated> notification, CancellationToken cancellationToken)
    {

    }
}

public record TeamUpdatedHandler(IApplicationDbContext Context, IEventPublisher EventPublisher) : INotificationHandler<DomainEventNotification<TeamUpdated>>
{
    public async Task Handle(DomainEventNotification<TeamUpdated> notification, CancellationToken cancellationToken)
    {

    }
}

public record TeamDeletedHandler(IApplicationDbContext Context, IEventPublisher EventPublisher) : INotificationHandler<DomainEventNotification<TeamDeleted>>
{
    public async Task Handle(DomainEventNotification<TeamDeleted> notification, CancellationToken cancellationToken)
    {

    }
}

public record TeamMemberAddedHandler(IEventPublisher EventPublisher) : INotificationHandler<DomainEventNotification<TeamMemberAdded>>
{
    public async Task Handle(DomainEventNotification<TeamMemberAdded> notification, CancellationToken cancellationToken)
    {
    }
}

public record TeamMemberRemovedHandler(IEventPublisher EventPublisher) : INotificationHandler<DomainEventNotification<TeamMemberRemoved>>
{
    public async Task Handle(DomainEventNotification<TeamMemberRemoved> notification, CancellationToken cancellationToken)
    {
    }
}