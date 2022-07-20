using MassTransit;

using MediatR;

using YourBrand.HumanResources.Contracts;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Organizations.Commands;

namespace YourBrand.TimeReport.Consumers;

public class TimeReportOrganizationCreatedConsumer : IConsumer<OrganizationCreated>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentOrganizationService;

    public TimeReportOrganizationCreatedConsumer(IMediator mediator, ICurrentUserService currentOrganizationService)
    {
        _mediator = mediator;
        _currentOrganizationService = currentOrganizationService;
    }

    public async Task Consume(ConsumeContext<OrganizationCreated> context)
    {
        var message = context.Message;

        _currentOrganizationService.SetCurrentUser(message.CreatedById);

        var result = await _mediator.Send(new CreateOrganizationCommand(message.OrganizationId, message.Name, null));
    }
}

public class TimeReportOrganizationDeletedConsumer : IConsumer<OrganizationDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentOrganizationService;

    public TimeReportOrganizationDeletedConsumer(IMediator mediator, ICurrentUserService currentOrganizationService)
    {
        _mediator = mediator;
        _currentOrganizationService = currentOrganizationService;
    }

    public async Task Consume(ConsumeContext<OrganizationDeleted> context)
    {
        var message = context.Message;

        _currentOrganizationService.SetCurrentUser(message.DeletedById);

        var result = await _mediator.Send(new DeleteOrganizationCommand(message.OrganizationId));
    }
}

public class TimeReportOrganizationUpdatedConsumer : IConsumer<OrganizationUpdated>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentOrganizationService;

    public TimeReportOrganizationUpdatedConsumer(IMediator mediator, ICurrentUserService currentOrganizationService)
    {
        _mediator = mediator;
        _currentOrganizationService = currentOrganizationService;
    }

    public async Task Consume(ConsumeContext<OrganizationUpdated> context)
    {
        var message = context.Message;

        _currentOrganizationService.SetCurrentUser(message.UpdatedById);

        var result = await _mediator.Send(new UpdateOrganizationCommand(message.OrganizationId, message.Name));
    }
}
