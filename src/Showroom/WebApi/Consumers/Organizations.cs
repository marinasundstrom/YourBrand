using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.Showroom.Application.Organizations.Commands;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Consumers;

public class ShowroomOrganizationCreatedConsumer(IMediator mediator) : IConsumer<OrganizationCreated>
{
    public async Task Consume(ConsumeContext<OrganizationCreated> context)
    {
        var message = context.Message;

        await mediator.Send(new CreateOrganizationCommand(message.OrganizationId, message.Name));
    }
}

public class ShowroomOrganizationDeletedConsumer(IMediator mediator) : IConsumer<OrganizationDeleted>
{
    public async Task Consume(ConsumeContext<OrganizationDeleted> context)
    {
        var message = context.Message;

        await mediator.Send(new DeleteOrganizationCommand(message.OrganizationId));
    }
}

public class ShowroomOrganizationUpdatedConsumer(IMediator mediator) : IConsumer<OrganizationUpdated>
{
    public async Task Consume(ConsumeContext<OrganizationUpdated> context)
    {
        var message = context.Message;

        await mediator.Send(new UpdateOrganizationCommand(message.OrganizationId, message.Name));
    }
}

public class ShowroomOrganizationUserAddedConsumer(IMediator mediator) : IConsumer<OrganizationUserAdded>
{
    public async Task Consume(ConsumeContext<OrganizationUserAdded> context)
    {
        var message = context.Message;

        await mediator.Send(new AddUserToOrganization(message.OrganizationId, message.UserId));
    }
}