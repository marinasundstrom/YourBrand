using YourBrand.Tenancy;

namespace YourBrand.Ticketing.Domain.Entities;

public sealed class TicketType : Entity<int>, IHasTenant
{
    protected TicketType() : base()
    {

    }

    public TicketType(string name) : base()
    {
        Name = name;
    }

    public TenantId TenantId { get; set; }

    public string Name { get; set; } = null!;
}