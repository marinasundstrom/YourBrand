using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public class MotionItem : Entity<MotionItemId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<Vote> _votes = new HashSet<Vote>();

    public MotionItem(string text)
    : base(new MotionItemId())
    {
        Text = text;
    }

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }
    public MotionId MotionId { get; set; }

    public string Text { get; set; }
    public int Order { get; set; }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}