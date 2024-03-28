using Microsoft.EntityFrameworkCore;

using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Domain.Entities;
using YourBrand.Messenger.Domain.Repositories;

namespace YourBrand.Messenger.Infrastructure.Persistence.Repositories;

sealed class ConversationRepository : IConversationRepository
{
    private readonly MessengerContext _context;

    public ConversationRepository(MessengerContext context)
    {
        _context = context;
    }

    public void AddConversation(Conversation conversation)
    {
        _context.Conversations.Add(conversation);
    }

    public async Task<Conversation?> GetConversation(string id, CancellationToken cancellationToken = default)
    {
        return await _context.Conversations
                .Include(c => c.Participants)
                .ThenInclude(c => c.User)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public IQueryable<Conversation> GetConversations()
    {
        return _context.Conversations
                .OrderByDescending(c => c.Created)
                .Include(c => c.Participants)
                .ThenInclude(c => c.User)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .AsSplitQuery();
    }

    public IQueryable<Conversation> GetConversationsForUser(string userId)
    {
        return _context.Conversations
                .OrderByDescending(c => c.Created)
                .Include(c => c.Participants)
                .ThenInclude(c => c.User)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .AsSplitQuery()
                .Where(c => c.Participants.Any(p => p.Id == userId));
    }
}
