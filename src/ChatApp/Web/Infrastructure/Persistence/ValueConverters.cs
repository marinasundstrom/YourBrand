using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ChatApp.Domain.ValueObjects;

namespace ChatApp.Infrastructure.Persistence.ValueConverters;

internal sealed class ChannelIdConverter : ValueConverter<ChannelId, Guid>
{
    public ChannelIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class MessageIdConverter : ValueConverter<MessageId, Guid>
{
    public MessageIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class UserIdConverter : ValueConverter<UserId, string>
{
    public UserIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}