using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct MeetingParticipantId
{
    public MeetingParticipantId(string value) => Value = value;

    public MeetingParticipantId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(MeetingParticipantId lhs, MeetingParticipantId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(MeetingParticipantId lhs, MeetingParticipantId rhs) => lhs.Value != rhs.Value;

    public static implicit operator MeetingParticipantId(string id) => new MeetingParticipantId(id);

    public static implicit operator MeetingParticipantId?(string? id) => id is null ? (MeetingParticipantId?)null : new MeetingParticipantId(id);

    public static implicit operator string(MeetingParticipantId id) => id.Value;

    public static bool TryParse(string? value, out MeetingParticipantId channelParticipantId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelParticipantId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out MeetingParticipantId channelParticipantId)
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