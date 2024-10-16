using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct MinutesAttendeeId
{
    public MinutesAttendeeId(string value) => Value = value;

    public MinutesAttendeeId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(MinutesAttendeeId lhs, MinutesAttendeeId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(MinutesAttendeeId lhs, MinutesAttendeeId rhs) => lhs.Value != rhs.Value;

    public static implicit operator MinutesAttendeeId(string id) => new MinutesAttendeeId(id);

    public static implicit operator MinutesAttendeeId?(string? id) => id is null ? (MinutesAttendeeId?)null : new MinutesAttendeeId(id);

    public static implicit operator string(MinutesAttendeeId id) => id.Value;

    public static bool TryParse(string? value, out MinutesAttendeeId channelAttendeeId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelAttendeeId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out MinutesAttendeeId channelAttendeeId)
    {
        if (value is null)
        {
            channelAttendeeId = default;
            return false;
        }

        channelAttendeeId = value;
        return true;
    }
}