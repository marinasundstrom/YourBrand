using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct ElectionId
{
    public ElectionId(string value) => Value = value;

    public ElectionId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(ElectionId lhs, ElectionId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(ElectionId lhs, ElectionId rhs) => lhs.Value != rhs.Value;

    public static implicit operator ElectionId(string id) => new ElectionId(id);

    public static implicit operator ElectionId?(string? id) => id is null ? (ElectionId?)null : new ElectionId(id);

    public static implicit operator string(ElectionId id) => id.Value;

    public static bool TryParse(string? value, out ElectionId channelAttendeeId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelAttendeeId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out ElectionId channelAttendeeId)
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