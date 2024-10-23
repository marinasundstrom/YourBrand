using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public record MeetingGroupId(int Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator MeetingGroupId(int id) => new MeetingGroupId(id);

    public static implicit operator int(MeetingGroupId id) => id.Value;

    public static implicit operator int?(MeetingGroupId id) => id?.Value;

    public static bool TryParse(int? value, out MeetingGroupId? channelId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelId);
    }

    public static bool TryParse(int? value, IFormatProvider? provider, out MeetingGroupId? channelId)
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