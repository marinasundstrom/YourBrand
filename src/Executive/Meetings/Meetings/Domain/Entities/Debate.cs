using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Meetings.Domain.ValueObjects;

namespace YourBrand.Meetings.Domain.Entities;

public class Debate : AggregateRoot<DebateId>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<DebateEntry> _entries = new HashSet<DebateEntry>();

    public TenantId TenantId { get; set; }
    public OrganizationId OrganizationId { get; set; }

    public MotionId MotionId { get; set; }
    public IReadOnlyCollection<DebateEntry> Entries => _entries;

    public bool IsDebateOpen { get; set; } = false;

    public void OpenDebate()
    {
        IsDebateOpen = true;
    }

    public void CloseDebate()
    {
        IsDebateOpen = false;
    }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}