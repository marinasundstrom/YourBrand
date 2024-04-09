using Microsoft.EntityFrameworkCore;

using YourBrand.Messenger.Domain.Entities;
using YourBrand.Messenger.Domain.Repositories;

namespace YourBrand.Messenger.Infrastructure.Persistence.Repositories;

sealed class ConversationRepository(MessengerContext context) : IConversationRepository
{
    public void AddConversation(Conversation conversation)
    {
        context.Conversations.Add(conversation);
    }

    public async Task<Conversation?> GetConversation(string id, CancellationToken cancellationToken = default)
    {
        return await context.Conversations
                .Include(c => c.Participants)
                .ThenInclude(c => c.User)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .AsSplitQuery()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public IQueryable<Conversation> GetConversations()
    {
        return context.Conversations
                .OrderByDescending(c => c.Created)
                .Include(c => c.Participants)
                .ThenInclude(c => c.User)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .AsSplitQuery();
    }

    public IQueryable<Conversation> GetConversationsForUser(string userId)
    {
        return context.Conversations
                .OrderByDescending(c => c.Created)
                .Include(c => c.Participants)
                .ThenInclude(c => c.User)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .AsSplitQuery()
                .Where(c => c.Participants.Any(p => p.Id == userId));
    }
}