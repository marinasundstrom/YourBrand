using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct ElectionCandidateId
{
    public ElectionCandidateId(string value) => Value = value;

    public ElectionCandidateId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(ElectionCandidateId lhs, ElectionCandidateId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(ElectionCandidateId lhs, ElectionCandidateId rhs) => lhs.Value != rhs.Value;

    public static implicit operator ElectionCandidateId(string id) => new ElectionCandidateId(id);

    public static implicit operator ElectionCandidateId?(string? id) => id is null ? (ElectionCandidateId?)null : new ElectionCandidateId(id);

    public static implicit operator string(ElectionCandidateId id) => id.Value;

    public static bool TryParse(string? value, out ElectionCandidateId channelAttendeeId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelAttendeeId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out ElectionCandidateId channelAttendeeId)
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
