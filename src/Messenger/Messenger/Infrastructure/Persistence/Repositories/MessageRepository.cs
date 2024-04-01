using Microsoft.EntityFrameworkCore;

using YourBrand.Messenger.Domain.Entities;
using YourBrand.Messenger.Domain.Repositories;

namespace YourBrand.Messenger.Infrastructure.Persistence.Repositories;

sealed class MessageRepository : IMessageRepository
{
    private readonly MessengerContext _context;

    public MessageRepository(MessengerContext context)
    {
        _context = context;
    }

    public IQueryable<Message> GetMessagesInConversation(string conversationId)
    {
        return _context.Messages.Where(m => m.ConversationId == conversationId);
    }

    public async Task<Message?> GetMessage(string messageId, CancellationToken cancellationToken = default)
    {
        return await _context.Messages.FirstOrDefaultAsync(m => m.Id == messageId, cancellationToken);
    }

    public void DeleteMessage(Message message)
    {
        _context.Remove(message);
    }
}