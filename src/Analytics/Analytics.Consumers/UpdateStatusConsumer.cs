//using MediatR;
namespace YourBrand.Analytics.Consumers;

/*
public sealed class UpdateStatusConsumer : IConsumer<UpdateStatus>
{
    private readonly IMediator mediator;

    public UpdateStatusConsumer(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public async Task Consume(ConsumeContext<UpdateStatus> context)
    {
        var message = context.Message;

        await mediator.Send(new Application.Todos.Commands.UpdateStatus(message.Id, (Application.Todos.Dtos.TodoStatusDto)message.Status));
    }
}
*/