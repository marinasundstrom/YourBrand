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

    public static bool TryParse(string? value, out SpeakerRequestId channelAttendeeId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelAttendeeId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out SpeakerRequestId channelAttendeeId)
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