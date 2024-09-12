using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Ticketing.Domain.ValueObjects;

public record TicketId(int Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator TicketId(int id) => new TicketId(id);

    public static implicit operator int(TicketId id) => id.Value;

    public static bool TryParse(int? value, out TicketId? channelId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelId);
    }

    public static bool TryParse(int? value, IFormatProvider? provider, out TicketId? channelId)
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