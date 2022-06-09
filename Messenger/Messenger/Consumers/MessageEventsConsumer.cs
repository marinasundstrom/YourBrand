using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Contracts;
using YourBrand.Messenger.Hubs;

using MassTransit;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Messenger.Consumers;

public class MessagePostedConsumer : IConsumer<MessagePosted>
{
    private readonly IMediator _mediator;
    private readonly IMessengerContext _messengerContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHubContext<MessageHub, IMessageClient> _hubContext;

    public MessagePostedConsumer(IMediator mediator, IMessengerContext messengerContext, ICurrentUserService currentUserService,
        IHubContext<MessageHub, IMessageClient> hubContext)
    {
        _mediator = mediator;
        _messengerContext = messengerContext;
        _currentUserService = currentUserService;
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<MessagePosted> context)
    {
        var message = context.Message.Message;

        var conversationId = message.ConversationId;
        var sentById = message.SentBy.Id;

        var participantUserId = await _messengerContext
            .ConversationParticipants
            .Where(x => x.Conversation.Id == conversationId && x.UserId != sentById) //Is not muted
            .Select(x => x.UserId)
            .ToArrayAsync();

        await _hubContext.Clients
            .Users(participantUserId)
            .MessageReceived(message);
    }
}

public class MessageUpdatedConsumer : IConsumer<MessageUpdated>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHubContext<MessageHub, IMessageClient> _hubContext;

    public MessageUpdatedConsumer(IMediator mediator, ICurrentUserService currentUserService,
        IHubContext<MessageHub, IMessageClient> hubContext)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<MessageUpdated> context)
    {
        var message = context.Message;

        await _hubContext.Clients.All.MessageEdited(new MessageEditedDto()
        {
            Id = message.MessageId,
            Text = message.Text,
            Edited = message.Edited
        });
    }
}

public class MessageDeletedConsumer : IConsumer<MessageDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHubContext<MessageHub, IMessageClient> _hubContext;

    public MessageDeletedConsumer(IMediator mediator, ICurrentUserService currentUserService,
        IHubContext<MessageHub, IMessageClient> hubContext)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<MessageDeleted> context)
    {
        var message = context.Message;

        await _hubContext.Clients.All.MessageDeleted(new MessageDeletedDto()
        {
            Id = message.MessageId
        });
    }
}

public class MessageReadConsumer : IConsumer<MessageRead>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IHubContext<MessageHub, IMessageClient> _hubContext;

    public MessageReadConsumer(IMediator mediator, ICurrentUserService currentUserService,
        IHubContext<MessageHub, IMessageClient> hubContext)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<MessageRead> context)
    {
        var message = context.Message;

        await _hubContext.Clients.All.MessageRead(message.Receipt!);
    }
}

//StartedTyping
//EndedTyping