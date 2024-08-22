using System.Diagnostics.CodeAnalysis;

namespace YourBrand.ChatApp.Domain.ValueObjects;

public struct ChannelParticipantId
{
    public ChannelParticipantId(Guid value) => Value = value;

    public ChannelParticipantId() => Value = Guid.NewGuid();

    public Guid Value { get; set; }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static bool operator ==(ChannelParticipantId lhs, ChannelParticipantId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(ChannelParticipantId lhs, ChannelParticipantId rhs) => lhs.Value != rhs.Value;

    public static implicit operator ChannelParticipantId(Guid id) => new ChannelParticipantId(id);

    public static implicit operator ChannelParticipantId?(Guid? id) => id is null ? (ChannelParticipantId?)null : new ChannelParticipantId(id.GetValueOrDefault());

    public static implicit operator Guid(ChannelParticipantId id) => id.Value;
}