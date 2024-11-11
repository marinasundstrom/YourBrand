using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct DiscussionId
{
    public DiscussionId(string value) => Value = value;

    public DiscussionId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(DiscussionId lhs, DiscussionId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(DiscussionId lhs, DiscussionId rhs) => lhs.Value != rhs.Value;

    public static implicit operator DiscussionId(string id) => new DiscussionId(id);

    public static implicit operator DiscussionId?(string? id) => id is null ? (DiscussionId?)null : new DiscussionId(id);

    public static implicit operator string(DiscussionId id) => id.Value;

    public static bool TryParse(string? value, out DiscussionId channelAttendeeId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelAttendeeId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out DiscussionId channelAttendeeId)
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