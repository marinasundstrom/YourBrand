
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Enums;

namespace YourBrand.TimeReport.Domain.Entities;

public class Absence : AuditableEntity<string>, IHasTenant, IHasOrganization, ISoftDeletableWithAudit<User>
{
    public Absence() : base(Guid.NewGuid().ToString())
    {
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; } = null!;

    public User User { get; set; } = null!;

    public Project? Project { get; set; }

    public AbsenceType Type { get; set; }

    public AbsenceStatus Status { get; set; }

    public DateOnly? Date { get; set; }

    public decimal Hours { get; set; }

    public bool FullDays { get; set; }

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    public string? Note { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public UserId? DeletedById { get; set; }

    public User? DeletedBy { get; set; }
}