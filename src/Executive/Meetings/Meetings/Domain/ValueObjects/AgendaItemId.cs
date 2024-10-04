using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public record AgendaItemId(int Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator AgendaItemId(int id) => new AgendaItemId(id);

    public static implicit operator int(AgendaItemId id) => id.Value;

    public static bool TryParse(int? value, out AgendaItemId? channelId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelId);
    }

    public static bool TryParse(int? value, IFormatProvider? provider, out AgendaItemId? channelId)
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