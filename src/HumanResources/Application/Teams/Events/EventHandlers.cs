using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Domain.Events;

namespace YourBrand.HumanResources.Application.Teams.Events;

public record TeamCreatedHandler(IApplicationDbContext Context, IEventPublisher EventPublisher) : IDomainEventHandler<TeamCreated>
{
    public async Task Handle(TeamCreated notification, CancellationToken cancellationToken)
    {

    }
}

public record TeamUpdatedHandler(IApplicationDbContext Context, IEventPublisher EventPublisher) : IDomainEventHandler<TeamUpdated>
{
    public async Task Handle(TeamUpdated notification, CancellationToken cancellationToken)
    {

    }
}

public record TeamDeletedHandler(IApplicationDbContext Context, IEventPublisher EventPublisher) : IDomainEventHandler<TeamDeleted>
{
    public async Task Handle(TeamDeleted notification, CancellationToken cancellationToken)
    {

    }
}

public record TeamMemberAddedHandler(IEventPublisher EventPublisher) : IDomainEventHandler<TeamMemberAdded>
{
    public async Task Handle(TeamMemberAdded notification, CancellationToken cancellationToken)
    {
    }
}

public record TeamMemberRemovedHandler(IEventPublisher EventPublisher) : IDomainEventHandler<TeamMemberRemoved>
{
    public async Task Handle(TeamMemberRemoved notification, CancellationToken cancellationToken)
    {
    }
}