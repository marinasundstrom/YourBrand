using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public record MotionId(int Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator MotionId(int id) => new MotionId(id);

    public static implicit operator int(MotionId id) => id.Value;

    public static implicit operator int?(MotionId id) => id?.Value;

    public static bool TryParse(int? value, out MotionId? channelId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelId);
    }

    public static bool TryParse(int? value, IFormatProvider? provider, out MotionId? channelId)
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