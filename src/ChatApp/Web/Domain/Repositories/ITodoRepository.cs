using ChatApp.Domain.ValueObjects;

namespace ChatApp.Domain.Repositories;

public interface IMessageRepository : IRepository<Message, MessageId>
{
}
