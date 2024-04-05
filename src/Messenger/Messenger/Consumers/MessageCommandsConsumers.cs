using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Application.Messages.Commands;
using YourBrand.Messenger.Contracts;

namespace YourBrand.Messenger.Consumers;

public class PostMessageConsumer : IConsumer<PostMessage>
{
    private readonly IMediator _mediator;
    private readonly IUserContext _userContext;
    private readonly IBus _bus;

    public PostMessageConsumer(IMediator mediator, IUserContext userContext, IBus bus)
    {
        _mediator = mediator;
        _userContext = userContext;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<PostMessage> context)
    {
        var message = context.Message;

        await _userContext.SetCurrentUserFromAccessTokenAsync(message.AccessToken);

        var result = await _mediator.Send(new PostMessageCommand(message.ConversationId, message.Text, message.ReplyToId));

        await context.RespondAsync<MessageDto>(result);
    }
}

public class UpdateMessageConsumer : IConsumer<UpdateMessage>
{
    private readonly IMediator _mediator;
    private readonly IUserContext _userContext;
    private readonly IBus _bus;

    public UpdateMessageConsumer(IMediator mediator, IUserContext userContext, IBus bus)
    {
        _mediator = mediator;
        _userContext = userContext;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<UpdateMessage> context)
    {
        var message = context.Message;

        await _userContext.SetCurrentUserFromAccessTokenAsync(message.AccessToken);

        await _mediator.Send(new UpdateMessageCommand(message.ConversationId, message.MessageId!, message.Text));
    }
}

public class DeleteMessageConsumer : IConsumer<DeleteMessage>
{
    private readonly IMediator _mediator;
    private readonly IUserContext _userContext;
    private readonly IBus _bus;

    public DeleteMessageConsumer(IMediator mediator, IUserContext userContext, IBus bus)
    {
        _mediator = mediator;
        _userContext = userContext;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<DeleteMessage> context)
    {
        var message = context.Message;

        await _userContext.SetCurrentUserFromAccessTokenAsync(message.AccessToken);

        await _mediator.Send(new DeleteMessageCommand(message.ConversationId, message.MessageId));
    }
}

public class MarkMessageAsReadConsumer : IConsumer<MarkMessageAsRead>
{
    private readonly IMediator _mediator;
    private readonly IUserContext _userContext;
    private readonly IBus _bus;

    public MarkMessageAsReadConsumer(IMediator mediator, IUserContext userContext, IBus bus)
    {
        _mediator = mediator;
        _userContext = userContext;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<MarkMessageAsRead> context)
    {
        var message = context.Message;

        await _userContext.SetCurrentUserFromAccessTokenAsync(message.AccessToken);

        await _mediator.Send(new SendMessageReceiptCommand(message.MessageId));
    }
}

public class StartTypingConsumer : IConsumer<StartTyping>
{
    private readonly IMediator _mediator;
    private readonly IUserContext _userContext;

    public StartTypingConsumer(IMediator mediator, IUserContext userContext)
    {
        _mediator = mediator;
        _userContext = userContext;
    }

    public async Task Consume(ConsumeContext<StartTyping> context)
    {
        var message = context.Message;

        await _userContext.SetCurrentUserFromAccessTokenAsync(message.AccessToken);

        //await _mediator.Send(new SendMessageReceiptCommand(message.MessageId));
    }
}

public class EndTypingConsumer : IConsumer<EndTyping>
{
    private readonly IMediator _mediator;
    private readonly IUserContext _userContext;

    public EndTypingConsumer(IMediator mediator, IUserContext userContext)
    {
        _mediator = mediator;
        _userContext = userContext;
    }

    public async Task Consume(ConsumeContext<EndTyping> context)
    {
        var message = context.Message;

        await _userContext.SetCurrentUserFromAccessTokenAsync(message.AccessToken);

        //await _mediator.Send(new SendMessageReceiptCommand(message.MessageId));
    }
}