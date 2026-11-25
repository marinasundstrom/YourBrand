
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Entities;

public class TaskCharge : AuditableEntity<string>, IHasTenant, IHasOrganization, ISoftDeletableWithAudit<User>
{
    public TaskCharge() : base(Guid.NewGuid().ToString())
    {
    }

    public TaskCharge(string? description)
    {
        Id = Guid.NewGuid().ToString();
        Description = description;
    }

    public string Id { get; private set; }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; } = null!;

    public ChargeType Type { get; set; }

    public Project? Project { get; set; }

    public Task? Task { get; set; }

    /// <summary>
    /// If added or overridden via a ProjectMembershipTask
    /// </summary>
    public ProjectMembershipTask? ProjectMembershipTask { get; set; }

    /// <summary>
    /// If overridden via a ProjectMembershipTask
    /// </summary>
    public TaskCharge? OverriddenCharge { get; set; }

    public ChargeBasis Basis { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public UserId? DeletedById { get; set; }

    public User? DeletedBy { get; set; }
}

public enum ChargeType
{
    Cost,
    Revenue
}

public enum ChargeBasis
{
    PerHour,
    PerOccurrence,
    PerTask
}