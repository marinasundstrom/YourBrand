using Microsoft.EntityFrameworkCore;

using YourBrand.Messenger.Domain.Entities;
using YourBrand.Messenger.Domain.Repositories;

namespace YourBrand.Messenger.Infrastructure.Persistence.Repositories;

sealed class MessageRepository(MessengerContext context) : IMessageRepository
{
    public IQueryable<Message> GetMessagesInConversation(string conversationId)
    {
        return context.Messages.Where(m => m.ConversationId == conversationId);
    }

    public async Task<Message?> GetMessage(string messageId, CancellationToken cancellationToken = default)
    {
        return await context.Messages.FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);
    }

    public void DeleteMessage(Message message)
    {
        context.Remove(message);
    }
}