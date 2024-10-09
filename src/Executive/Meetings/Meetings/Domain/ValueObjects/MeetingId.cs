using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public record MeetingId(int Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator MeetingId(int id) => new MeetingId(id);

    public static implicit operator int(MeetingId id) => id.Value;

    public static implicit operator int?(MeetingId id) => id?.Value;

    public static bool TryParse(int? value, out MeetingId? channelId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelId);
    }

    public static bool TryParse(int? value, IFormatProvider? provider, out MeetingId? channelId)
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