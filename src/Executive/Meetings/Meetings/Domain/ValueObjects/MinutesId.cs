using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public record MinutesId(int Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator MinutesId(int id) => new MinutesId(id);

    public static implicit operator int(MinutesId id) => id.Value;

    public static implicit operator int?(MinutesId id) => id?.Value;

    public static bool TryParse(int? value, out MinutesId? channelId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelId);
    }

    public static bool TryParse(int? value, IFormatProvider? provider, out MinutesId? channelId)
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
