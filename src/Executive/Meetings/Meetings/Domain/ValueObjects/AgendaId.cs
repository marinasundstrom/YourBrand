using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public record AgendaId(int Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator AgendaId(int id) => new AgendaId(id);

    public static implicit operator int(AgendaId id) => id.Value;

    public static implicit operator int?(AgendaId id) => id?.Value;

    public static bool TryParse(int? value, out AgendaId? channelId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelId);
    }

    public static bool TryParse(int? value, IFormatProvider? provider, out AgendaId? channelId)
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