using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct ElectionSessionId
{
    public ElectionSessionId(string value) => Value = value;

    public ElectionSessionId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(ElectionSessionId lhs, ElectionSessionId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(ElectionSessionId lhs, ElectionSessionId rhs) => lhs.Value != rhs.Value;

    public static implicit operator ElectionSessionId(string id) => new ElectionSessionId(id);

    public static implicit operator ElectionSessionId?(string? id) => id is null ? (ElectionSessionId?)null : new ElectionSessionId(id);

    public static implicit operator string(ElectionSessionId id) => id.Value;

    public static bool TryParse(string? value, out ElectionSessionId channelAttendeeId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelAttendeeId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out ElectionSessionId channelAttendeeId)
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