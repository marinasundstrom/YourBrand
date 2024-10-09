using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public record DebateEntryId(int Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator DebateEntryId(int id) => new DebateEntryId(id);

    public static implicit operator int(DebateEntryId id) => id.Value;

    public static implicit operator int?(DebateEntryId id) => id?.Value;

    public static bool TryParse(int? value, out DebateEntryId? channelId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelId);
    }

    public static bool TryParse(int? value, IFormatProvider? provider, out DebateEntryId? channelId)
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
