using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct MeetingAttendeeFunctionId
{
    public MeetingAttendeeFunctionId(string value) => Value = value;

    public MeetingAttendeeFunctionId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(MeetingAttendeeFunctionId lhs, MeetingAttendeeFunctionId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(MeetingAttendeeFunctionId lhs, MeetingAttendeeFunctionId rhs) => lhs.Value != rhs.Value;

    public static implicit operator MeetingAttendeeFunctionId(string id) => new MeetingAttendeeFunctionId(id);

    public static implicit operator MeetingAttendeeFunctionId?(string? id) => id is null ? (MeetingAttendeeFunctionId?)null : new MeetingAttendeeFunctionId(id);

    public static implicit operator string(MeetingAttendeeFunctionId id) => id.Value;

    public static bool TryParse(string? value, out MeetingAttendeeFunctionId channelAttendeeFunctionId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelAttendeeFunctionId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out MeetingAttendeeFunctionId channelAttendeeFunctionId)
    {
        if (value is null)
        {
            channelAttendeeFunctionId = default;
            return false;
        }

        channelAttendeeFunctionId = value;
        return true;
    }
}
