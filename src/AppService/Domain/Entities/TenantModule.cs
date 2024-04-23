using YourBrand.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Domain.Entities;

public sealed class TenantModule : Entity, IHasTenant
{
    public Guid Id { get; set; } = default!;

    public TenantId TenantId { get; set; }

    public Module Module { get; set; }

    public bool Enabled { get; set; }
}