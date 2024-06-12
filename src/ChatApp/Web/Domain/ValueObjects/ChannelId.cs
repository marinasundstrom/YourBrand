using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Domain.ValueObjects;

public struct ChannelId
{
    public ChannelId(Guid value) => Value = value;

    public ChannelId() => Value = Guid.NewGuid();

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

    public static bool operator ==(ChannelId lhs, ChannelId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(ChannelId lhs, ChannelId rhs) => lhs.Value != rhs.Value;

    public static implicit operator ChannelId(Guid id) => new ChannelId(id);

    public static implicit operator ChannelId?(Guid? id) => id is null ? (ChannelId?)null : new ChannelId(id.GetValueOrDefault());

    public static implicit operator Guid(ChannelId id) => id.Value;
}
