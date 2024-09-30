using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Ticketing.Domain.ValueObjects;

public record ProjectId(int Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator ProjectId(int id) => new ProjectId(id);

    public static implicit operator int(ProjectId id) => id.Value;

    public static bool TryParse(int? value, out ProjectId? channelId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelId);
    }

    public static bool TryParse(int? value, IFormatProvider? provider, out ProjectId? channelId)
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