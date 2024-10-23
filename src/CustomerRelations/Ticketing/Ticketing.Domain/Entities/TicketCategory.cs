using YourBrand.Domain;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

using OrganizationId = YourBrand.Domain.OrganizationId;

namespace YourBrand.Ticketing.Domain.Entities;

public sealed class TicketCategory : Entity<int>, IHasTenant, IHasOrganization
{
    protected TicketCategory() : base()
    {

    }

    public TicketCategory(int id, string name) : base()
    {
        Id = id;
        Name = name;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public ProjectId ProjectId { get; set; }

    public TicketType? TicketType { get; set; }

    public int? TicketTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public TicketCategory? Parent { get; set; }

    public int? ParentId { get; set; }

    public HashSet<TicketCategory> SubCategories { get; set; } = new HashSet<TicketCategory>();
}