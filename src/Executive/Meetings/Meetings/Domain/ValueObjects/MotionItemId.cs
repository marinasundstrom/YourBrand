using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct MotionItemId
{
    public MotionItemId(string value) => Value = value;

    public MotionItemId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(MotionItemId lhs, MotionItemId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(MotionItemId lhs, MotionItemId rhs) => lhs.Value != rhs.Value;

    public static implicit operator MotionItemId(string id) => new MotionItemId(id);

    public static implicit operator MotionItemId?(string? id) => id is null ? (MotionItemId?)null : new MotionItemId(id);

    public static implicit operator string(MotionItemId id) => id.Value;

    public static bool TryParse(string? value, out MotionItemId channelParticipantId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelParticipantId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out MotionItemId channelParticipantId)
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