using MassTransit;

using MediatR;

using YourBrand.Ticketing.Contracts;

namespace YourBrand.Ticketing.Consumers;

public sealed class UpdateStatusConsumer(IMediator mediator) : IConsumer<UpdateStatus>
{
    public async Task Consume(ConsumeContext<UpdateStatus> context)
    {
        var message = context.Message;

        //await mediator.Send(new Application.Orders.Commands.UpdateStatus(message, null!));
    }
}