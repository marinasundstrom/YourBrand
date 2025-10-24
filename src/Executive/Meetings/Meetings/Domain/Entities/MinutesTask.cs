using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public enum MinutesTaskType
{
    AdjustMinutes,
    ApproveMinutes
}

public enum MinutesTaskStatus
{
    Pending,
    Completed,
    Cancelled
}

public sealed class MinutesTask : Entity<MinutesTaskId>, IAuditableEntity<MinutesTaskId>, IHasTenant, IHasOrganization
{
    public MinutesTask()
        : base(new MinutesTaskId())
    {
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public MinutesId MinutesId { get; set; }

    public Minutes? Minutes { get; set; }

    public MinutesTaskType Type { get; set; }

    public MinutesTaskStatus Status { get; private set; } = MinutesTaskStatus.Pending;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public UserId? AssignedToId { get; set; }

    public User? AssignedTo { get; set; }

    public string? AssignedToName { get; set; }

    public string? AssignedToEmail { get; set; }

    public DateTimeOffset AssignedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? DueAt { get; set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public UserId? CompletedById { get; private set; }

    public User? CompletedBy { get; set; }

    public void Complete(UserId completedBy, DateTimeOffset completedAt)
    {
        if (Status == MinutesTaskStatus.Completed)
        {
            return;
        }

        Status = MinutesTaskStatus.Completed;
        CompletedById = completedBy;
        CompletedAt = completedAt;
    }

    public void Reopen()
    {
        Status = MinutesTaskStatus.Pending;
        CompletedAt = null;
        CompletedById = null;
    }

    public void Cancel()
    {
        Status = MinutesTaskStatus.Cancelled;
    }

    public User? CreatedBy { get; set; } = null!;

    public UserId? CreatedById { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}
