using System.Diagnostics.CodeAnalysis;

namespace YourBrand.Tenancy;

public struct TenantId
{
    public TenantId(string value) => Value = value;

    public string Value { get; private set; }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return Value?.GetHashCode() ?? string.Empty.GetHashCode();
    }

    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }

    public static bool operator ==(TenantId lhs, TenantId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(TenantId lhs, TenantId rhs) => lhs.Value != rhs.Value;

    public static implicit operator TenantId(string id) => id is null ? default : new TenantId(id);

    public static implicit operator TenantId?(string? id) => id is null ? (TenantId?)null : new TenantId(id);

    public static implicit operator string(TenantId id) => id.Value;
}
