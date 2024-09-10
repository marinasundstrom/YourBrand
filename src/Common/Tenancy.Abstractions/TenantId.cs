using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Tenancy;

public struct TenantId(string value)
{
    public string Value { get; private set; } = value;

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

    public static bool TryParse(string? value, out TenantId? tenantId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out tenantId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out TenantId? tenantId)
    {
        if (value is null)
        {
            tenantId = default;
            return false;
        }

        tenantId = value;
        return true;
    }
}