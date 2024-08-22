using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.ChatApp.Domain.ValueObjects;

public record ChannelId(Guid Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator ChannelId(Guid id) => new ChannelId(id);

    public static implicit operator ChannelId?(Guid? id) => id is null ? (ChannelId?)null : new ChannelId(id.GetValueOrDefault());

    public static implicit operator Guid(ChannelId id) => id.Value;

    public static bool TryParse(string? value, out ChannelId? channelId) 
    { 
        return TryParse(value, CultureInfo.CurrentCulture, out channelId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out ChannelId? channelId)
    {
        if (value is null)
        {
            channelId = default;
            return false;
        }

        Guid cid;
        var r = Guid.TryParse(value, provider, out cid);
        if (!r)
        {
            channelId = default;
            return false;
        }
        channelId = cid;
        return true;
    }
}