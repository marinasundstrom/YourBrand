using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct MinutesItemId
{
    public MinutesItemId(string value) => Value = value;

    public MinutesItemId() => Value = Guid.NewGuid().ToString();

    public string Value { get; set; }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return (Value ?? string.Empty).GetHashCode();
    }

    public override string ToString()
    {
        return (Value ?? string.Empty).ToString();
    }

    public static bool operator ==(MinutesItemId lhs, MinutesItemId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(MinutesItemId lhs, MinutesItemId rhs) => lhs.Value != rhs.Value;

    public static implicit operator MinutesItemId(string id) => new MinutesItemId(id);

    public static implicit operator MinutesItemId?(string? id) => id is null ? (MinutesItemId?)null : new MinutesItemId(id);

    public static implicit operator string(MinutesItemId id) => id.Value;

    public static bool TryParse(string? value, out MinutesItemId channelParticipantId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelParticipantId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out MinutesItemId channelParticipantId)
    {
        if (value is null)
        {
            channelParticipantId = default;
            return false;
        }

        channelParticipantId = value;
        return true;
    }
}