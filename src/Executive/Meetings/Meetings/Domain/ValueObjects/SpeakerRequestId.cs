using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct SpeakerRequestId
{
    public SpeakerRequestId(string value) => Value = value;

    public SpeakerRequestId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(SpeakerRequestId lhs, SpeakerRequestId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(SpeakerRequestId lhs, SpeakerRequestId rhs) => lhs.Value != rhs.Value;

    public static implicit operator SpeakerRequestId(string id) => new SpeakerRequestId(id);

    public static implicit operator SpeakerRequestId?(string? id) => id is null ? (SpeakerRequestId?)null : new SpeakerRequestId(id);

    public static implicit operator string(SpeakerRequestId id) => id.Value;

    public static bool TryParse(string? value, out SpeakerRequestId channelParticipantId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelParticipantId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out SpeakerRequestId channelParticipantId)
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