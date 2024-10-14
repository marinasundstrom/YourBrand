using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct MeetingGroupMemberId
{
    public MeetingGroupMemberId(string value) => Value = value;

    public MeetingGroupMemberId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(MeetingGroupMemberId lhs, MeetingGroupMemberId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(MeetingGroupMemberId lhs, MeetingGroupMemberId rhs) => lhs.Value != rhs.Value;

    public static implicit operator MeetingGroupMemberId(string id) => new MeetingGroupMemberId(id);

    public static implicit operator MeetingGroupMemberId?(string? id) => id is null ? (MeetingGroupMemberId?)null : new MeetingGroupMemberId(id);

    public static implicit operator string(MeetingGroupMemberId id) => id.Value;

    public static bool TryParse(string? value, out MeetingGroupMemberId channelParticipantId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelParticipantId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out MeetingGroupMemberId channelParticipantId)
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