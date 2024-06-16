using YourBrand.ChatApp.Domain.ValueObjects;

namespace YourBrand.ChatApp.Domain.Repositories;

public interface IMessageRepository : IRepository<Message, MessageId>
{
}