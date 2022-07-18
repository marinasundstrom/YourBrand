using MassTransit;

using MediatR;

using YourBrand.HumanResources.Contracts;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Teams.Commands;

namespace YourBrand.TimeReport.Consumers;

public class TimeReportTeamCreatedConsumer : IConsumer<TeamCreated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetTeam> _requestClient;
    private readonly ICurrentUserService _currentTeamService;

    public TimeReportTeamCreatedConsumer(IMediator mediator, IRequestClient<GetTeam> requestClient, ICurrentUserService currentTeamService)
    {
        _mediator = mediator;
        _requestClient = requestClient;
        _currentTeamService = currentTeamService;
    }

    public async Task Consume(ConsumeContext<TeamCreated> context)
    {
        var message = context.Message;

        _currentTeamService.SetCurrentUser(message.CreatedById);

        var result = await _mediator.Send(new CreateTeamCommand(message.TeamId, message.OrganizationId, message.Name, message.Description));
    }
}

public class TimeReportTeamDeletedConsumer : IConsumer<TeamDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentTeamService;

    public TimeReportTeamDeletedConsumer(IMediator mediator, ICurrentUserService currentTeamService)
    {
        _mediator = mediator;
        _currentTeamService = currentTeamService;
    }

    public async Task Consume(ConsumeContext<TeamDeleted> context)
    {
        var message = context.Message;

        _currentTeamService.SetCurrentUser(message.DeletedById);

        var result = await _mediator.Send(new DeleteTeamCommand(message.TeamId));
    }
}

public class TimeReportTeamUpdatedConsumer : IConsumer<TeamUpdated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetTeam> _requestClient;
    private readonly ICurrentUserService _currentTeamService;

    public TimeReportTeamUpdatedConsumer(IMediator mediator, IRequestClient<GetTeam> requestClient, ICurrentUserService currentTeamService)
    {
        _mediator = mediator;
        _requestClient = requestClient;
        _currentTeamService = currentTeamService;
    }

    public async Task Consume(ConsumeContext<TeamUpdated> context)
    {
        var message = context.Message;

        _currentTeamService.SetCurrentUser(message.UpdatedById);

        var messageR = await _requestClient.GetResponse<GetTeamResponse>(new GetTeam(message.TeamId, (message.UpdatedById)));
        var message2 = messageR.Message;

        var result = await _mediator.Send(new UpdateTeamCommand(message2.TeamId, message.Name, message.Description));
    }
}
