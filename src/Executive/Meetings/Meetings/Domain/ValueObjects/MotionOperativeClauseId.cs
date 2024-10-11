using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct MotionOperativeClauseId
{
    public MotionOperativeClauseId(string value) => Value = value;

    public MotionOperativeClauseId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(MotionOperativeClauseId lhs, MotionOperativeClauseId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(MotionOperativeClauseId lhs, MotionOperativeClauseId rhs) => lhs.Value != rhs.Value;

    public static implicit operator MotionOperativeClauseId(string id) => new MotionOperativeClauseId(id);

    public static implicit operator MotionOperativeClauseId?(string? id) => id is null ? (MotionOperativeClauseId?)null : new MotionOperativeClauseId(id);

    public static implicit operator string(MotionOperativeClauseId id) => id.Value;

    public static bool TryParse(string? value, out MotionOperativeClauseId channelParticipantId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelParticipantId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out MotionOperativeClauseId channelParticipantId)
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