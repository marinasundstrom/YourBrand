using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public record DebateId(int Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator DebateId(int id) => new DebateId(id);

    public static implicit operator int(DebateId id) => id.Value;

    public static bool TryParse(int? value, out DebateId? channelId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelId);
    }

    public static bool TryParse(int? value, IFormatProvider? provider, out DebateId? channelId)
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
