using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct MeetingAttendeeId
{
    public MeetingAttendeeId(string value) => Value = value;

    public MeetingAttendeeId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(MeetingAttendeeId lhs, MeetingAttendeeId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(MeetingAttendeeId lhs, MeetingAttendeeId rhs) => lhs.Value != rhs.Value;

    public static implicit operator MeetingAttendeeId(string id) => new MeetingAttendeeId(id);

    public static implicit operator MeetingAttendeeId?(string? id) => id is null ? (MeetingAttendeeId?)null : new MeetingAttendeeId(id);

    public static implicit operator string(MeetingAttendeeId id) => id.Value;

    public static bool TryParse(string? value, out MeetingAttendeeId channelAttendeeId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelAttendeeId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out MeetingAttendeeId channelAttendeeId)
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