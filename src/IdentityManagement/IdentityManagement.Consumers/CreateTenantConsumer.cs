using MassTransit;

using MediatR;

using YourBrand.IdentityManagement.Application.Tenants.Commands;
using YourBrand.IdentityManagement.Contracts;

namespace YourBrand.IdentityManagement.Consumers;

public class CreateTenantConsumer(IMediator mediator) : IConsumer<CreateTenant>
{
    public async Task Consume(ConsumeContext<CreateTenant> context)
    {
        var message = context.Message;

        var tenant = await mediator.Send(new CreateTenantCommand(message.Name, message.FriendlyName));

        await context.RespondAsync(new CreateTenantResponse(tenant.Id));
    }
}