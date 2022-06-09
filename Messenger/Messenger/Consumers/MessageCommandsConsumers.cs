using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Application.Messages.Commands;
using YourBrand.Messenger.Contracts;

using MassTransit;

using MediatR;

namespace YourBrand.Messenger.Consumers;

public class PostMessageConsumer : IConsumer<PostMessage>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBus _bus;

    public PostMessageConsumer(IMediator mediator, ICurrentUserService currentUserService, IBus bus)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<PostMessage> context)
    {
        var message = context.Message;

        await _currentUserService.SetCurrentUserFromAccessTokenAsync(message.AccessToken);

        var result = await _mediator.Send(new PostMessageCommand(message.ConversationId, message.Text, message.ReplyToId));

        await context.RespondAsync<MessageDto>(result);
    }
}

public class UpdateMessageConsumer : IConsumer<UpdateMessage>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBus _bus;

    public UpdateMessageConsumer(IMediator mediator, ICurrentUserService currentUserService, IBus bus)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<UpdateMessage> context)
    {
        var message = context.Message;

        await _currentUserService.SetCurrentUserFromAccessTokenAsync(message.AccessToken);

        await _mediator.Send(new UpdateMessageCommand(message.MessageId!, message.Text));
    }
}

public class DeleteMessageConsumer : IConsumer<DeleteMessage>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBus _bus;

    public DeleteMessageConsumer(IMediator mediator, ICurrentUserService currentUserService, IBus bus)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<DeleteMessage> context)
    {
        var message = context.Message;

        await _currentUserService.SetCurrentUserFromAccessTokenAsync(message.AccessToken);

        await _mediator.Send(new DeleteMessageCommand(message.MessageId));
    }
}

public class MarkMessageAsReadConsumer : IConsumer<MarkMessageAsRead>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBus _bus;

    public MarkMessageAsReadConsumer(IMediator mediator, ICurrentUserService currentUserService, IBus bus)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<MarkMessageAsRead> context)
    {
        var message = context.Message;

        await _currentUserService.SetCurrentUserFromAccessTokenAsync(message.AccessToken);

        await _mediator.Send(new SendMessageReceiptCommand(message.MessageId));
    }
}

public class StartTypingConsumer : IConsumer<StartTyping>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public StartTypingConsumer(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<StartTyping> context)
    {
        var message = context.Message;

        await _currentUserService.SetCurrentUserFromAccessTokenAsync(message.AccessToken);

        //await _mediator.Send(new SendMessageReceiptCommand(message.MessageId));
    }
}

public class EndTypingConsumer : IConsumer<EndTyping>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public EndTypingConsumer(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<EndTyping> context)
    {
        var message = context.Message;

        await _currentUserService.SetCurrentUserFromAccessTokenAsync(message.AccessToken);

        //await _mediator.Send(new SendMessageReceiptCommand(message.MessageId));
    }
}
