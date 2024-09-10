using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.ChatApp.Domain.ValueObjects;

public record ChannelId(string Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator ChannelId(string id) => new ChannelId(id);

    public static implicit operator string(ChannelId id) => id.Value;

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

        channelId = value;
        return true;
    }
}