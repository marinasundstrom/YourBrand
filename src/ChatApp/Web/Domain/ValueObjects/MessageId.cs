using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Domain.ValueObjects;

public struct MessageId
{
    public MessageId(Guid value) => Value = value;

    public MessageId() => Value = Guid.NewGuid();

    public Guid Value { get; set; } 

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return base.Equals(obj);
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static bool operator ==(MessageId lhs, MessageId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(MessageId lhs, MessageId rhs) => lhs.Value != rhs.Value;

    public static implicit operator MessageId(Guid id) => new MessageId(id);

    public static implicit operator MessageId?(Guid? id) => id is null ? (MessageId?)null : new MessageId(id.GetValueOrDefault());

    public static implicit operator Guid(MessageId id) => id.Value;
}
