using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct SpeakerSessionId
{
    public SpeakerSessionId(string value) => Value = value;

    public SpeakerSessionId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(SpeakerSessionId lhs, SpeakerSessionId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(SpeakerSessionId lhs, SpeakerSessionId rhs) => lhs.Value != rhs.Value;

    public static implicit operator SpeakerSessionId(string id) => new SpeakerSessionId(id);

    public static implicit operator SpeakerSessionId?(string? id) => id is null ? (SpeakerSessionId?)null : new SpeakerSessionId(id);

    public static implicit operator string(SpeakerSessionId id) => id.Value;

    public static bool TryParse(string? value, out SpeakerSessionId channelParticipantId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelParticipantId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out SpeakerSessionId channelParticipantId)
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
