using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Application.Users.Commands;

namespace YourBrand.TimeReport.Consumers;

public class TimeReportUserCreatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient, ITenantContext tenantContext, IUserContext userContext) : IConsumer<UserCreated>
{
    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var message = context.Message;

        tenantContext.SetTenantId(message.TenantId);
        userContext.SetCurrentUser(message.CreatedById);

        var messageR = await requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId, message.CreatedById));
        var message2 = messageR.Message;

        var result = await mediator.Send(new CreateUserCommand(message2.UserId, message2.OrganizationId, message2.FirstName, message2.LastName, message2.DisplayName, "SSN", message2.Email));
    }
}

public class TimeReportUserDeletedConsumer(IMediator mediator, ITenantContext tenantContext, IUserContext userContext) : IConsumer<UserDeleted>
{
    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;

        //_tenantContext.SetTenantId(message.TenantId);
        userContext.SetCurrentUser(message.DeletedById);

        await mediator.Send(new DeleteUserCommand(message.UserId));
    }
}

public class TimeReportUserUpdatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient, ITenantContext tenantContext, IUserContext userContext) : IConsumer<UserUpdated>
{
    public async Task Consume(ConsumeContext<UserUpdated> context)
    {
        var message = context.Message;

        //tenantContext.SetTenantId(message.TenantId);
        userContext.SetCurrentUser(message.UpdatedById);

        var messageR = await requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId, (message.UpdatedById)));
        var message2 = messageR.Message;

        var result = await mediator.Send(new UpdateUserCommand(message2.UserId, message2.FirstName, message2.LastName, message2.DisplayName, "SSN", message2.Email));
    }
}