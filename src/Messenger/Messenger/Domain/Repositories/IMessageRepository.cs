
using YourBrand.Messenger.Domain.Entities;

namespace YourBrand.Messenger.Domain.Repositories;

public interface IMessageRepository
{
    IQueryable<Message> GetMessagesInConversation(string conversationId);
    Task<Message?> GetMessage(string messageId, CancellationToken cancellationToken = default);
    void DeleteMessage(Message message);
}