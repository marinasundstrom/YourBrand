﻿
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Entities;

public class Task : AuditableEntity<string>, IHasTenant, IHasOrganization, ISoftDeletableWithAudit<User>
{
    public Task() : base(Guid.NewGuid().ToString())
    {
    }

    public Task(string name, TaskType taskType, string? description)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        TaskType = taskType;
        Description = description;
    }

    public string Id { get; set; } = null!;

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public TaskType TaskType { get; set; } = null!;

    public string? Description { get; set; }

    public Project Project { get; set; } = null!;

    public List<Entry> Entries { get; set; } = new List<Entry>();

    /// <summary>
    /// Minimum hours per day / entry
    /// </summary>
    public double? MinHoursPerDay { get; set; }

    /// <summary>
    /// Maximum hours per day / entry
    /// </summary>
    public double? MaxHoursPerDay { get; set; }

    /// <summary>
    /// Hourly rate. Positive value = Revenue and Negative value = Cost
    /// </summary>
    public decimal? HourlyRate { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public UserId? DeletedById { get; set; }

    public User? DeletedBy { get; set; }
}