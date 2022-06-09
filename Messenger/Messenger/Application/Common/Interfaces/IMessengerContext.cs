using YourBrand.Messenger.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Messenger.Application.Common.Interfaces;

public interface IMessengerContext
{
    DbSet<Conversation> Conversations { get; }

    DbSet<ConversationParticipant> ConversationParticipants { get; }

    DbSet<Message> Messages { get; }

    DbSet<MessageReceipt> MessageReceipts { get; }

    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<ITransaction> BeginTransactionAsync();
}

public interface ITransaction : IDisposable, IAsyncDisposable
{
    Task CommitAsync();
    Task RollbackAsync();
}