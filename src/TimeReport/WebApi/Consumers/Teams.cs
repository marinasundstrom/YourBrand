using MassTransit;

using MediatR;

using YourBrand.HumanResources.Contracts;
using YourBrand.Identity;
using YourBrand.TimeReport.Application.Teams.Commands;

namespace YourBrand.TimeReport.Consumers;

public class TimeReportTeamCreatedConsumer(IMediator mediator, IRequestClient<GetTeam> requestClient) : IConsumer<TeamCreated>
{
    public async Task Consume(ConsumeContext<TeamCreated> context)
    {
        var message = context.Message;

        var result = await mediator.Send(new CreateTeamCommand(message.TeamId, message.OrganizationId, message.Name, message.Description));
    }
}

public class TimeReportTeamDeletedConsumer(IMediator mediator) : IConsumer<TeamDeleted>
{
    public async Task Consume(ConsumeContext<TeamDeleted> context)
    {
        var message = context.Message;

        await mediator.Send(new DeleteTeamCommand(message.TeamId));
    }
}

public class TimeReportTeamUpdatedConsumer(IMediator mediator, IRequestClient<GetTeam> requestClient) : IConsumer<TeamUpdated>
{
    public async Task Consume(ConsumeContext<TeamUpdated> context)
    {
        var message = context.Message;

        var messageR = await requestClient.GetResponse<GetTeamResponse>(new GetTeam(message.TeamId));
        var message2 = messageR.Message;

        var result = await mediator.Send(new UpdateTeamCommand(message2.TeamId, message.Name, message.Description));
    }
}

public class TimeReportTeamMemberAddedConsumer(IMediator mediator, IRequestClient<GetTeam> requestClient) : IConsumer<TeamMemberAdded>
{
    public async Task Consume(ConsumeContext<TeamMemberAdded> context)
    {
        var message = context.Message;

        await mediator.Send(new AddTeamMemberCommand(message.TeamId, message.PersonId));
    }
}