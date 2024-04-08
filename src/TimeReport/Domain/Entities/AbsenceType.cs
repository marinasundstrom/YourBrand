
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;

namespace YourBrand.TimeReport.Domain.Entities;

public class AbsenceType : AuditableEntity, IHasTenant, ISoftDelete
{
    public string Id { get; set; } = null!;

    public TenantId TenantId { get; set; }

    public string Name { get; set; } = null!;

    public bool FullDays { get; set; }

    public List<Absence> Absences { get; set; } = new List<Absence>();

    public DateTime? Deleted { get; set; }

    public UserId? DeletedById { get; set; }

    public User? DeletedBy { get; set; }
}