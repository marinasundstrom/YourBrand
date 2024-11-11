using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct VotingId
{
    public VotingId(string value) => Value = value;

    public VotingId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(VotingId lhs, VotingId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(VotingId lhs, VotingId rhs) => lhs.Value != rhs.Value;

    public static implicit operator VotingId(string id) => new VotingId(id);

    public static implicit operator VotingId?(string? id) => id is null ? (VotingId?)null : new VotingId(id);

    public static implicit operator string(VotingId id) => id.Value;

    public static bool TryParse(string? value, out VotingId channelAttendeeId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelAttendeeId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out VotingId channelAttendeeId)
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