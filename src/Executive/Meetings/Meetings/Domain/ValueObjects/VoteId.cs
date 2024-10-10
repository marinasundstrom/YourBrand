using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct VoteId
{
    public VoteId(string value) => Value = value;

    public VoteId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(VoteId lhs, VoteId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(VoteId lhs, VoteId rhs) => lhs.Value != rhs.Value;

    public static implicit operator VoteId(string id) => new VoteId(id);

    public static implicit operator VoteId?(string? id) => id is null ? (VoteId?)null : new VoteId(id);

    public static implicit operator string(VoteId id) => id.Value;

    public static bool TryParse(string? value, out VoteId channelParticipantId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelParticipantId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out VoteId channelParticipantId)
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