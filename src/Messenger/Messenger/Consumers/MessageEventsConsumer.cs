using MassTransit;

using MediatR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Contracts;
using YourBrand.Messenger.Hubs;

namespace YourBrand.Messenger.Consumers;

public sealed class MessagePostedConsumer(IMessengerContext messengerContext,
    IHubContext<MessageHub, IMessageClient> hubContext) : IConsumer<MessagePosted>
{
    public async Task Consume(ConsumeContext<MessagePosted> context)
    {
        var message = context.Message.Message;

        var conversationId = message.ConversationId;
        var sentById = message.SentBy.Id;

        var participantUserId = await messengerContext
            .ConversationParticipants
            .Where(x => x.Conversation.Id == conversationId && x.UserId != sentById) //Is not muted
            .Select(x => x.UserId)
            .ToArrayAsync();

        await hubContext.Clients
            .Users(participantUserId)
            .MessageReceived(message);
    }
}

public sealed class MessageUpdatedConsumer(
    IHubContext<MessageHub, IMessageClient> hubContext) : IConsumer<MessageUpdated>
{
    public async Task Consume(ConsumeContext<MessageUpdated> context)
    {
        var message = context.Message;

        await hubContext.Clients.All.MessageEdited(new MessageEditedDto()
        {
            Id = message.MessageId,
            Text = message.Text,
            Edited = message.Edited
        });
    }
}

public sealed class MessageDeletedConsumer(
    IHubContext<MessageHub, IMessageClient> hubContext) : IConsumer<MessageDeleted>
{
    public async Task Consume(ConsumeContext<MessageDeleted> context)
    {
        var message = context.Message;

        await hubContext.Clients.All.MessageDeleted(new MessageDeletedDto()
        {
            Id = message.MessageId
        });
    }
}

public sealed class MessageReadConsumer(
    IHubContext<MessageHub, IMessageClient> hubContext) : IConsumer<MessageRead>
{
    public async Task Consume(ConsumeContext<MessageRead> context)
    {
        var message = context.Message;

        await hubContext.Clients.All.MessageRead(message.Receipt!);
    }
}

//StartedTyping
//EndedTyping