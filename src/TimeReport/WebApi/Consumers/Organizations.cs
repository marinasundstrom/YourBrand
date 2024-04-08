using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Application.Organizations.Commands;

namespace YourBrand.TimeReport.Consumers;

public class TimeReportOrganizationCreatedConsumer(IMediator mediator) : IConsumer<OrganizationCreated>
{
    public async Task Consume(ConsumeContext<OrganizationCreated> context)
    {
        var message = context.Message;

        var result = await mediator.Send(new CreateOrganizationCommand(message.OrganizationId, message.Name, null));
    }
}

public class TimeReportOrganizationDeletedConsumer(IMediator mediator) : IConsumer<OrganizationDeleted>
{
    public async Task Consume(ConsumeContext<OrganizationDeleted> context)
    {
        var message = context.Message;

        await mediator.Send(new DeleteOrganizationCommand(message.OrganizationId));
    }
}

public class TimeReportOrganizationUpdatedConsumer(IMediator mediator) : IConsumer<OrganizationUpdated>
{
    public async Task Consume(ConsumeContext<OrganizationUpdated> context)
    {
        var message = context.Message;

        var result = await mediator.Send(new UpdateOrganizationCommand(message.OrganizationId, message.Name));
    }
}

public class TimeReportOrganizationUserAddedConsumer(IMediator mediator) : IConsumer<OrganizationUserAdded>
{
    public async Task Consume(ConsumeContext<OrganizationUserAdded> context)
    {
        var message = context.Message;

        var result = await mediator.Send(new AddUserToOrganization(message.OrganizationId, message.UserId));
    }
}