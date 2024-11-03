using YourBrand.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Domain.Entities;

public sealed class TenantModule : Entity<Guid>, IHasTenant
{
    public TenantId TenantId { get; set; }

    public Module Module { get; set; }

    public bool Enabled { get; set; }
}