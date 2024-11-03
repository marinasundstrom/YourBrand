
using System.Data.SqlTypes;

using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Entities;

public class AbsenceType : AuditableEntity<string>, IHasTenant, IHasOrganization, ISoftDeletable
{

    public AbsenceType() : base(Guid.NewGuid().ToString())
    {
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool FullDays { get; set; }

    public List<Absence> Absences { get; set; } = new List<Absence>();

    public bool IsDeleted { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public UserId? DeletedById { get; set; }

    public User? DeletedBy { get; set; }
}