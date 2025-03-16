using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Users.Commands;
using YourBrand.IdentityManagement.Contracts;

namespace YourBrand.IdentityManagement.Consumers;

public class IsEmailAlreadyRegisteredConsumer(IMediator mediator) : IConsumer<IsEmailAlreadyRegistered>
{
    public async Task Consume(ConsumeContext<IsEmailAlreadyRegistered> context)
    {
        var message = context.Message;

        var res = await mediator.Send(new IsEmailAlreadyRegisteredCommand(message.Email));

        await context.RespondAsync(new IsEmailAlreadyRegisteredResponse() {
            IsEmailRegistered = res.EmailExists
        });
    }
}