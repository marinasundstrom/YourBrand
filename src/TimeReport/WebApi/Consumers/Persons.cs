using MassTransit;

using MediatR;

using YourBrand.HumanResources.Contracts;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Application.Users.Commands;

namespace YourBrand.TimeReport.Consumers;

public class TimeReportPersonCreatedConsumer(IMediator mediator, IRequestClient<GetPerson> requestClient) : IConsumer<PersonCreated>
{
    public async Task Consume(ConsumeContext<PersonCreated> context)
    {
        var message = context.Message;

        var messageR = await requestClient.GetResponse<GetPersonResponse>(new GetPerson(message.PersonId, message.CreatedById));
        var message2 = messageR.Message;

        var result = await mediator.Send(new CreateUserCommand(message2.PersonId, message2.OrganizationId, message2.FirstName, message2.LastName, message2.DisplayName, "SSN", message2.Email));
    }
}

public class TimeReportPersonDeletedConsumer(IMediator mediator) : IConsumer<PersonDeleted>
{
    public async Task Consume(ConsumeContext<PersonDeleted> context)
    {
        var message = context.Message;

        await mediator.Send(new DeleteUserCommand(message.PersonId));
    }
}

public class TimeReportPersonUpdatedConsumer(IMediator mediator, IRequestClient<GetPerson> requestClient) : IConsumer<PersonUpdated>
{
    public async Task Consume(ConsumeContext<PersonUpdated> context)
    {
        var message = context.Message;

        var messageR = await requestClient.GetResponse<GetPersonResponse>(new GetPerson(message.PersonId, (message.UpdatedById)));
        var message2 = messageR.Message;

        var result = await mediator.Send(new UpdateUserCommand(message2.PersonId, message2.FirstName, message2.LastName, message2.DisplayName, "SSN", message2.Email));
    }
}