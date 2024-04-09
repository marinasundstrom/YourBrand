using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Application.Messages.Commands;
using YourBrand.Messenger.Contracts;

namespace YourBrand.Messenger.Consumers;

public class PostMessageConsumer(IMediator mediator, IBus bus) : IConsumer<PostMessage>
{
    private readonly ISettableUserContext _userContext;

    public async Task Consume(ConsumeContext<PostMessage> context)
    {
        var message = context.Message;

        var result = await mediator.Send(new PostMessageCommand(message.ConversationId, message.Text, message.ReplyToId));

        await context.RespondAsync<MessageDto>(result);
    }
}

public class UpdateMessageConsumer(IMediator mediator, IBus bus) : IConsumer<UpdateMessage>
{
    public async Task Consume(ConsumeContext<UpdateMessage> context)
    {
        var message = context.Message;

        await mediator.Send(new UpdateMessageCommand(message.ConversationId, message.MessageId!, message.Text));
    }
}

public class DeleteMessageConsumer(IMediator mediator, IBus bus) : IConsumer<DeleteMessage>
{
    public async Task Consume(ConsumeContext<DeleteMessage> context)
    {
        var message = context.Message;

        await mediator.Send(new DeleteMessageCommand(message.ConversationId, message.MessageId));
    }
}

public class MarkMessageAsReadConsumer(IMediator mediator, ISettableUserContext userContext, IBus bus) : IConsumer<MarkMessageAsRead>
{
    public async Task Consume(ConsumeContext<MarkMessageAsRead> context)
    {
        var message = context.Message;

        await userContext.SetCurrentUserFromAccessTokenAsync(message.AccessToken);

        await mediator.Send(new SendMessageReceiptCommand(message.MessageId));
    }
}

public class StartTypingConsumer(IMediator mediator, ISettableUserContext userContext) : IConsumer<StartTyping>
{
    public async Task Consume(ConsumeContext<StartTyping> context)
    {
        var message = context.Message;

        await userContext.SetCurrentUserFromAccessTokenAsync(message.AccessToken);

        //await _mediator.Send(new SendMessageReceiptCommand(message.MessageId));
    }
}

public class EndTypingConsumer(IMediator mediator, ISettableUserContext userContext) : IConsumer<EndTyping>
{
    public async Task Consume(ConsumeContext<EndTyping> context)
    {
        var message = context.Message;

        await userContext.SetCurrentUserFromAccessTokenAsync(message.AccessToken);

        //await _mediator.Send(new SendMessageReceiptCommand(message.MessageId));
    }
}