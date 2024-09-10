using YourBrand.ChatApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.ChatApp.Domain;

public static class MessageExtensions
{
    public static IQueryable<T> InChannel<T>(this IQueryable<T> query, ChannelId channelId)
        where T : IHasChannel
    {
        return query.Where(x => x.ChannelId == channelId);
    }

    public static IEnumerable<T> InOrganization<T>(this IEnumerable<T> query, ChannelId channelId)
      where T : IHasChannel
    {
        return query.Where(x => x.ChannelId == channelId);
    }

    public static async Task<Message?> GetMessageAsync<T>(this IQueryable<Message> query, MessageId messageId, CancellationToken cancellationToken = default)
    {
        return await query.FirstOrDefaultAsync(x => x.Id == messageId, cancellationToken);
    }

    public static Message? GetMessage(this IEnumerable<Message> query, MessageId messageId)
    {
        return query.FirstOrDefault(x => x.Id == messageId);
    }
}