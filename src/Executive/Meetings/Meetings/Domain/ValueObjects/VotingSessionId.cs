using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct VotingSessionId
{
    public VotingSessionId(string value) => Value = value;

    public VotingSessionId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(VotingSessionId lhs, VotingSessionId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(VotingSessionId lhs, VotingSessionId rhs) => lhs.Value != rhs.Value;

    public static implicit operator VotingSessionId(string id) => new VotingSessionId(id);

    public static implicit operator VotingSessionId?(string? id) => id is null ? (VotingSessionId?)null : new VotingSessionId(id);

    public static implicit operator string(VotingSessionId id) => id.Value;

    public static bool TryParse(string? value, out VotingSessionId channelAttendeeId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelAttendeeId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out VotingSessionId channelAttendeeId)
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