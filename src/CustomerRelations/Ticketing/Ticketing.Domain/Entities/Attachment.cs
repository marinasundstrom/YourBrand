using YourBrand.Tenancy;

namespace YourBrand.Ticketing.Domain.Entities;

public class Attachment : Entity<int>, IHasTenant
{
    public TenantId TenantId { get; set; }

    public string Name { get; set; } = null!;
}