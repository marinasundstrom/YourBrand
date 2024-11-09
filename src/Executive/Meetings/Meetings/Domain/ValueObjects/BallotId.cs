using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct BallotId
{
    public BallotId(string value) => Value = value;

    public BallotId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(BallotId lhs, BallotId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(BallotId lhs, BallotId rhs) => lhs.Value != rhs.Value;

    public static implicit operator BallotId(string id) => new BallotId(id);

    public static implicit operator BallotId?(string? id) => id is null ? (BallotId?)null : new BallotId(id);

    public static implicit operator string(BallotId id) => id.Value;

    public static bool TryParse(string? value, out BallotId channelAttendeeId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelAttendeeId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out BallotId channelAttendeeId)
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