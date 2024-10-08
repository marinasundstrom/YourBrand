using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using YourBrand.ChatApp.Domain.ValueObjects;

namespace YourBrand.ChatApp.Infrastructure.Persistence.ValueConverters;

internal sealed class ChannelIdConverter : ValueConverter<ChannelId, string>
{
    public ChannelIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class ChannelParticipantIdConverter : ValueConverter<ChannelParticipantId, string>
{
    public ChannelParticipantIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}

internal sealed class MessageIdConverter : ValueConverter<MessageId, string>
{
    public MessageIdConverter()
        : base(v => v.Value, v => new(v))
    {
    }
}