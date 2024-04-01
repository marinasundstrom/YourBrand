
using YourBrand.Messenger.Domain.Entities;

namespace YourBrand.Messenger.Domain.Repositories;

public interface IConversationRepository
{
    Task<Conversation?> GetConversation(string id, CancellationToken cancellationToken = default);

    IQueryable<Conversation> GetConversations();

    IQueryable<Conversation> GetConversationsForUser(string userId);

    void AddConversation(Conversation conversation);
}