using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public record VoteId(int Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator VoteId(int id) => new VoteId(id);

    public static implicit operator int(VoteId id) => id.Value;

    public static implicit operator int?(VoteId id) => id?.Value;
    
    public static bool TryParse(int? value, out VoteId? channelId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelId);
    }

    public static bool TryParse(int? value, IFormatProvider? provider, out VoteId? channelId)
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