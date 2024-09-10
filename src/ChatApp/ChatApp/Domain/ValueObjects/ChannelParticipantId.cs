using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.ChatApp.Domain.ValueObjects;

public struct ChannelParticipantId
{
    public ChannelParticipantId(string value) => Value = value;

    public ChannelParticipantId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(ChannelParticipantId lhs, ChannelParticipantId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(ChannelParticipantId lhs, ChannelParticipantId rhs) => lhs.Value != rhs.Value;

    public static implicit operator ChannelParticipantId(string id) => new ChannelParticipantId(id);

    public static implicit operator ChannelParticipantId?(string? id) => id is null ? (ChannelParticipantId?)null : new ChannelParticipantId(id);

    public static implicit operator string(ChannelParticipantId id) => id.Value;

    public static bool TryParse(string? value, out ChannelParticipantId channelParticipantId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelParticipantId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out ChannelParticipantId channelParticipantId)
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